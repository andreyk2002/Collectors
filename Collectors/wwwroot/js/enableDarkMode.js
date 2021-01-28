const darkmode = new Darkmode();
if (darkmode.isActivated()) {
    document.getElementById("dark-toggle").checked = true;
}
else {
    document.getElementById("dark-toggle").checked = false;
}