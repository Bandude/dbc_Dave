using dbc_Dave.Data.Models;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dbc_Dave.Pages
{
    partial class Index
    {

        // DeleteQuery:
        // Prompts the user for confirmation before deleting a query associated with a specified option.
        // Communicates with a JavaScript function to obtain user confirmation before making a call to
        // the "DeleteQuery" function of the Redis service with the current username.
        private async Task DeleteQuery(string option)
        {
            try
            {
                var confirmed = await JSRuntime.InvokeAsync<bool>("confirmDelete");
                if (confirmed)
                {
                    await redis.DeleteQuery(option, UserName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }


        // Handles the process of saving the current query configuration.
        // If the current query is not "New...", it prompts the user to confirm overwriting the existing configuration,
        // or to enter a new configuration name if they don't want to overwrite.
        // If the current query is "New...", it simply prompts the user to enter a configuration name.
        // Once the user inputs a valid configuration name, or confirms to overwrite, it serializes the
        // message object to JSON and stores it in Redis under the combined key of configuration name and username.
        private async void SaveConfig()
        {


            // Check if currentQuery is not "New..."
            if (currentQuery != "New...")
            {
                // Prompt user for confirmation to overwrite
                var overwrite = await JSRuntime.InvokeAsync<bool>("confirm", "This will overwrite the existing configuration. Are you sure you want to continue?");
                if (!overwrite)
                {
                    // User chose not to overwrite, return without saving
                    var configName = await JSRuntime.InvokeAsync<string>("prompt", "Enter The Config Name:");

                    while (configName == "")
                    {
                        configName = await JSRuntime.InvokeAsync<string>("prompt", "You can not enter an empty name, press cancel or provide a valid name:");
                    }

                    DataQuery query = new DataQuery();
                    query.QueryName = configName;
                    query.QueryText = JsonConvert.SerializeObject(messages);
                    query.UserId = UserName;


                    if (string.IsNullOrEmpty(configName))
                    {
                        // User cancelled the prompt, return without saving
                        return;
                    }


                    await redis.SetValue(configName + ":" + UserName, JsonConvert.SerializeObject(messages), query, UserName);
                }
                else //if overwrite
                {

                    var configName = currentQuery;
                    DataQuery query = new DataQuery();
                    query.QueryName = configName;
                    query.QueryText = JsonConvert.SerializeObject(messages);
                    query.UserId = UserName;

                    await redis.SetValue(configName + ":" + UserName, JsonConvert.SerializeObject(messages), query, UserName);

                }

            }
            else
            {
                // User chose not to overwrite, return without saving
                var configName = await JSRuntime.InvokeAsync<string>("prompt", "Enter The Config Name:");



                while (configName == "")
                {
                    configName = await JSRuntime.InvokeAsync<string>("prompt", "You can not enter an empty name, press cancel or provide a valid name:");

                    if (string.IsNullOrEmpty(configName))
                    {
                        // User cancelled the prompt, return without saving
                        return;
                    }
                }




                DataQuery query = new DataQuery();
                query.QueryName = configName;
                query.QueryText = JsonConvert.SerializeObject(messages);
                query.UserId = UserName;
                currentQuery = configName ;

                await redis.SetValue(currentQuery + ":" + UserName, JsonConvert.SerializeObject(messages), query, UserName);

            }

            //load the query just saved
            await GetQuery(currentQuery, false);
      

            StateHasChanged();

        }

        // Retrieves and sets the messages list for a given query ID.
        // If the provided query ID is "New...", it clears all messages and sets the currentQuery to "New...".
        // Otherwise, it fetches the stored messages from Redis using the query ID and username as the key,
        // deserializes the messages, and updates the UI to display the retrieved messages.
        private async Task GetQuery(string queryId, bool hideGrid = true)
        {
            try
            {
                if (queryId == "New...")
                {
                    ClearAllMessages();
                    currentQuery = "New...";
                    return;
                }
                var key = queryId + ":" + UserName;
                var messagesNew = JsonConvert.DeserializeObject<List<DaveMessage>>(await redis.GetValue(key));
                await redis.SetValue("current" + UserName, JsonConvert.SerializeObject(messagesNew));
                messages = await redis.GetOrCreateMessagesAsync("current" + UserName);
                currentQuery = queryId ?? "New...";
 
                if (hideGrid)
                {

                    await JSRuntime.InvokeVoidAsync("showGrid");
                }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }


        // Adds a new message to the messages list and handles system prompts.
        // Checks if there is a system prompt to add at the beginning, and ensures the total word count
        // of all messages doesn't exceed a word limit, removing older messages if necessary.
        // Adds the new message based on the provided role and content, then serializes the updated messages list back to Redis.
        // Resets the message input box and potentially fetches a reply if required.
        private async Task AddMessage(string? role, bool execute)
        {
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                DaveMessage systemKeyPair = new DaveMessage("system", systemPrompt);
                if (messages == null)
                {
                    //var messages = await redis.GetOrCreateMessagesAsync("current" + UserName);

                }
                else
                {
                    if (messages.Count == 0 || messages[0].Role != "system")
                    {
                        messages.Insert(0, systemKeyPair);
                        await JSRuntime.InvokeVoidAsync("clearTextarea", "sysprmpt");
                        ShowSystemTextarea = false;
                    }
                }

            }
            else
            {
                // If there are no system prompts, ensure the system textarea is visible.
                ShowSystemTextarea = true;
            }



            if (!string.IsNullOrEmpty(message))
            {
                if (messages.Count > 0)
                {
                    int wordCount = messages.Sum(x => x.Content.Split(' ').Length);
                    if (wordCount >= 7000)
                    {
                        if (messages[0].Role == "system")
                        {
                            messages.RemoveAt(1);
                        }
                        else
                        {
                            messages.RemoveAt(0);
                        }
                    }
                }

                if (role == "assistant" || role == "user")
                {
                    messages.Add(new DaveMessage(selectedRole, message));
                }
                else if (role == "system")
                {
                    messages.Insert(1, new DaveMessage(selectedRole, message));
                    // Clear the system prompt message textarea after adding the system message.
                    await JSRuntime.InvokeVoidAsync("clearTextarea", "sysprmpt");
                    ShowSystemTextarea = false;
                }



                if (controlEnter)
                {
                    GetReply();
                    controlEnter = false;
                }
                StateHasChanged();
                message = "";
                await JSRuntime.InvokeVoidAsync("resetMessageBox");

                await redis.SetValue("current" + UserName, JsonConvert.SerializeObject(messages));



            }
        }

        // Toggles the edit mode for a specific message at a given index.
        // If exiting edit mode, calls a utility to update the text area height based on the message content.
        private async void UpdateMessage(int index)
        {
            if (messages[index].EditMode)
            {
                messages[index].EditMode = false;
                string messageid = $"textArea-{index}";
                await Utility.UpdateTextAreaHeight(messageid);

            }
        }

        // Removes a message at a specified index from the messages list and updates the stored messages in Redis.
        private void RemoveMessage(int index)
        {

            messages.RemoveAt(index);
            redis.SetValue("current" + UserName, JsonConvert.SerializeObject(messages));


        }

        // Clears all messages from the UI and updates the Redis store to reflect this change.
        private void ClearAllMessages()
        {
            messages.Clear();
            redis.SetValue("current" + UserName, JsonConvert.SerializeObject(messages));
        }



        // Initiates a request to the external OpenAI Chat API to get a reply based on the current messages in the conversation.
        // Shows a loading indicator during the operation, adds the reply to the messages list, and updates the UI accordingly.
        // It also handles setting the necessary state if the operation is cancelled or if an error occurs.
        private async void GetReply()
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();

                var conversationList = messages.Select(m => new KeyValuePair<string, string>(m.Role, m.Content)).ToList();

                isLoading = true;
                var content = await openAiApi.ChatCompletionsAsync(selectedModel, conversationList, cancellationTokenSource.Token);
                JObject jsonObject = JObject.Parse(content);
                content = jsonObject["choices"]?[0]?["message"]?["content"]?.ToString() ?? "";
                messages.Add(new DaveMessage("assistant", content));
                isLoading = false;
                StateHasChanged();
                await HighlightCodeBlocks();
                await Task.Delay(200);
                await JSRuntime.InvokeVoidAsync("resetMessageBox");
                await redis.SetValue("current" + UserName, JsonConvert.SerializeObject(messages));

            }
            catch (OperationCanceledException)
            {
                isLoading = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                isLoading = false;
                errorMessage = "There was an error: " + ex.Message;
                showErrorPopup = true;
            }
        }
    }
}
