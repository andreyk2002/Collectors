let tags = document.getElementsByClassName('tags');
let inputSearch = document.getElementById('tagSearch')

for (let i = 0; i < tags.length; i++) {
    tags[i].addEventListener('click', function () {
        inputSearch.value = tags[i].innerHTML;
    });  
}