const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/comments")
    .build();
let itemId = '0';
let userName = '';
hubConnection.on("Send", function (message, userName) {
    if (message.trim() === '')
        return;
    apendMessageToHtml(userName, message);
});

document.getElementById("sendBtn").addEventListener("click", function (e) {
    let message = document.getElementById("message").value;
    userName = document.getElementById("userName").value;
    itemId = document.getElementById("itemId").value;
    hubConnection.invoke("Send", message, userName, itemId);
});


hubConnection.start();

function apendMessageToHtml(userName, message) {
    let chatElement = document.createElement("div");
    chatElement.classList.add("message");
    let userNameElem = document.createElement("b");
    userNameElem.appendChild(document.createTextNode(userName + ': '));
    let elem = document.createElement("p");
    buildMessage(elem, userNameElem, message, chatElement);
    let firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(chatElement, firstElem);
}


function buildMessage(elem, userNameElem, message, chatElement) {
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));
    chatElement.appendChild(elem);
}
