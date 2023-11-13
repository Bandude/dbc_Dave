using dbc_Dave.Data.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Web;

namespace dbc_Dave.Pages
{
    partial class Index
    {





        private async Task UpdateTextAreaHeight(string id)
        {
            await JSRuntime.InvokeVoidAsync("resizeTextArea", id);
        }


        private int GetRowCount(string content)
        {
            return content.Split('\n').Length;
        }


   


      
        private int GetTokenCount()
        {
            int wordCount = messages.Sum(x => x.Content.Split(' ').Length);
            int tokenCount = (int)Math.Ceiling(wordCount * 0.75);
            return tokenCount;
        }


        public async Task HighlightCodeBlocks()
        {
            await JSRuntime.InvokeVoidAsync("highlightAllCodeBlocks");
        }

        public async Task codeBlockCopy(string codeblockid)
        {
            await JSRuntime.InvokeVoidAsync("copyCode", codeblockid);
        }

        protected async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.CtrlKey && e.Key == "Enter")
            {
                controlEnter = true;
                await hiddenInputElement.FocusAsync(); // Focus on hidden input to defocus the textarea

            }
        }

        private void CancelReply()
        {
            cancellationTokenSource.Cancel();
        }




    }



}
