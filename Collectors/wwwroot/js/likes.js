let likes = document.getElementsByClassName('like');
let likesCounts = document.getElementById('likesCount')

for (let i = 0; i < tags.length; i++) {
    tags[i].addEventListener('click', function () {
        likesCounts[i].innerHTML++;
    });
}