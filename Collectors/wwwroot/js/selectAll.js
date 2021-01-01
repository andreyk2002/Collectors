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

//unblockButton.addEventListener('click', function () {
//    alert('This function is not implemented yet');
//})

//blockButton.addEventListener('click', function () {
//    alert('This function is not implemented yet');
//})

//deleteButton.addEventListener('click', function () {
//    alert('This function is not implemented yet');
//})