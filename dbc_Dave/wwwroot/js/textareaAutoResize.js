window.resizeTextArea = (id) => {
    element = document.getElementById(id);
    element.style.height = (element.scrollHeight) + "px";
    element.style.overflow = "hidden";
    element.style.resize = "none";
};

function highlightAllCodeBlocks() {
    document.querySelectorAll('pre code').forEach((block) => {
        hljs.highlightElement(block);
    });
}

function  resetMessageBox() {
    const messageBox = document.getElementById('newmessage');
    messageBox.style.height = "80px";
    console.log("Reset Message Box");
    setTimeout(function () {
        scrollToBottom();
        if (messageBox) {
            element.focus();
        }
    }, 200);

}


window.setFocusOnElement = function (id) {
    var element = document.getElementById(id);
    if (element) {
        element.focus();
    }
}


function handleButtonClick(event, codeBlockId) {
    event.stopPropagation();
    var preTag = document.querySelector(`code[id='codeBlock${codeBlockId}']`);
    var textToCopy = preTag.textContent || preTag.innerText;

    // Creating a temporary textarea to copy the text
    var tempTextarea = document.createElement('textarea');
    tempTextarea.value = textToCopy;
    document.body.appendChild(tempTextarea);
    tempTextarea.select();
    document.execCommand('copy');
    document.body.removeChild(tempTextarea);


}



function scrollToBottom() {
    window.scrollTo({
        top: document.documentElement.scrollHeight,
        behavior: 'smooth'
    });

}

window.setFocusOnElement = function (id) {
    var element = document.getElementById(id);
    if (element) {
        element.focus();
    }
};


function showGrid() {
    // Get the grid element
    var grid = document.getElementById("cardGrid");

    // Get the button element
    var button = document.getElementById("toggleButton");

    // Check if grid is currently visible
    var isGridVisible = !grid.classList.contains('d-none');

    // If the grid is visible, hide it and update button text
    if (isGridVisible) {
        grid.classList.add('d-none');
        button.innerHTML = "Load";
    }
    // If the grid is not visible, show it and update button text
    else {
        grid.classList.remove('d-none');
        button.innerHTML = "Hide grid";
    }
}