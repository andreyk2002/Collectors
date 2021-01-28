'use strict'

let checkItAll = document.getElementById('selectAll');
let inputs = document.getElementsByClassName('checkbox-item');
let editButton = document.getElementById('editButton');

checkItAll.addEventListener('change', function () {
    for (let i = 0; i < inputs.length; i++){
        inputs[i].checked = checkItAll.checked;
    }
});

editButton.addEventListener('click', function (event) {
    let checked = 0;
    for (let i = 0; i < inputs.length; i++) {
        if (inputs[i].checked)
            checked++;
    }
    if (checked > 1) {
        event.preventDefault();
        event.stopPropagation();
        alert("Can't edit multiple items at once");
    }
});

let buttons = document.getElementsByClassName('additional-field');
let inputForIndex = document.getElementById('fieldIndex');
for (let i = 0; i < buttons.length; i++) {
    buttons[i].addEventListener('click', function () {
        inputForIndex.value = i;
    });  
}
