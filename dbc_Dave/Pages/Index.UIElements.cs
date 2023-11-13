using Microsoft.JSInterop;
using System.Text;
using System.Web;

namespace dbc_Dave.Pages
{
    partial class Index
    {

        // Toggles the selected AI model between "gpt-4-1106-preview" and "gpt-3.5-turbo-0613" based on a checkbox input.
        private void ToggleModel(bool isChecked)
        {
            selectedModel = isChecked ? "gpt-4-1106-preview" : "gpt-3.5-turbo-0613";
        }

        // Switches the current role within the user interface between 'user' and 'assistant'.
        private void ToggleRole()
        {
            selectedRole = selectedRole == "user" ? "assistant" : "user";
        }


        // Returns the appropriate CSS class for a role button, and updates the display name based on the selected role.
        // The role parameter can be "user", "assistant", or "system".
        private string GetRoleButtonClass(string role)
        {
            if (role == "assistant")
            {
                selectedRoleDisplayName = "Dave";
            }
            else
            {
                selectedRoleDisplayName = "You";
            }

            return role switch
            {
                "user" => "btn-user",
                "assistant" => "btn-assistant",
                "system" => "btn-system",
                _ => "btn-secondary",
            };
        }

        // Closes the currently displayed error popup by hiding it from the user interface.

        private void CloseErrorPopup()
        {
            showErrorPopup = false;
        }

        // Enters the edit mode for a message at the given index in the messages list,
        // triggers a UI refresh, and sets focus to the message's text area for editing.
        private async Task ChangeEditMode(int index)
        {
            messages[index].EditMode = true;
            StateHasChanged();
            await Task.Delay(50);
            string messageid = $"textArea-{index}";
            await UpdateTextAreaHeight(messageid);
            await JSRuntime.InvokeVoidAsync("setFocusOnElement", messageid);
        }


        // Renders the input content into formatted HTML, handling markdown code blocks and converting them into highlighted HTML code elements.
        // Code blocks are delimited by triple backticks (```), and the method keeps track of codeBlockCount for unique element IDs.
        private async Task CopyMessageContent(string content)
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", content);
        }

        private string RenderCodeBlock(string content)
        {
            string[] lines = content.Split('\n');
            StringBuilder sb = new StringBuilder();

            bool inCodeBlock = false;
            string codeLanguage = "";



            foreach (string line in lines)
            {
                if (line.StartsWith("```"))
                {
                    codeBlockCount++;
                    inCodeBlock = !inCodeBlock;

                    if (inCodeBlock)
                    {
                        codeLanguage = line.Substring(3).Trim();
                        sb.AppendLine($"<pre><div class='copy-code-container'><button data-copy-code-button onclick='handleButtonClick(event, {codeBlockCount})' class='copy-code-button'> Copy </button ></div><code id = 'codeBlock{codeBlockCount}' class='hljs'>");
                    }
                    else
                    {

                        sb.AppendLine($"</code></pre>");
                        codeLanguage = "";
                    }
                }
                else
                {
                    if (line != "")
                    {
                        sb.AppendLine(inCodeBlock ? HttpUtility.HtmlEncode(line) : $"<p class=\"aiText\">{line}</p>");
                    }

                }
            }

            return sb.ToString();
        }

    }
}
