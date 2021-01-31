'use strict'

let checkItAll = document.getElementById('selectAll');
let inputs = document.getElementsByClassName('checkbox-item');

checkItAll.addEventListener('change', function () {
    for (let i = 0; i < inputs.length; i++){
        inputs[i].checked = checkItAll.checked;
    }
});

let buttons = document.getElementsByClassName('additional-field');
let inputForIndex = document.getElementById('fieldIndex');
for (let i = 0; i < buttons.length; i++) {
    buttons[i].addEventListener('click', function () {
        inputForIndex.value = i;
    });  
}
