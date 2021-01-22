'use strict'

let dataTable = document.getElementById('data-table');
let checkItAll = dataTable.querySelector('input[name="select_all"]');
let inputs = dataTable.querySelectorAll('tbody>tr>td>input');
let deleteButton = document.getElementById('Delete');
let blockButton = document.getElementById('Block');
let unblockButton = document.getElementById('Unblock');

checkItAll.addEventListener('change', function () {
    inputs.forEach(function (input) {
        input.checked = checkItAll.checked;
    });
});

editButton.addEventListener('click', function (event) {
    let checked = 0;
    inputs.forEach(i => {
        if (i.checked)
            checked++;
    });
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
