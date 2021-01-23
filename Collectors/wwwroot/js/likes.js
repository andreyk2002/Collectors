let likes = document.getElementsByClassName('like');
let likesCounts = document.getElementsByClassName('likesCount');

for (let i = 0; i < likes.length; i++) {
    likes[i].addEventListener('click', function () {
        likesCounts[i].innerHTML++;
    });
}