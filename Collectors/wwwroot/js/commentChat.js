const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/comments")
    .build();
let itemId = '0';
let userName = '';
hubConnection.on("Send", function (message, userName) {

    let userNameElem = document.createElement("b");
    userNameElem.appendChild(document.createTextNode(userName + ': '));

    let elem = document.createElement("p");
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));
    let firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);

});

document.getElementById("sendBtn").addEventListener("click", function (e) {
    let message = document.getElementById("message").value;
    userName = document.getElementById("userName").value;
    itemId = document.getElementById("itemId").value;
    hubConnection.invoke("Send", message, userName, itemId);
});


hubConnection.start();