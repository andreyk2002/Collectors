let input = document.querySelector('input[name="input-custom-dropdown"]');
let tagsArea = document.getElementById('tags');
let tags = tagsArea.value.split(',');
let sentButton = document.getElementById('sentButton');
let tagify = new Tagify(input, {
    whitelist: tags,
    blacklist: ["Something"],
    maxTags: 6,
    dropdown: {
        maxItems: 20,
        classname: "tags-look",
        enabled: 0,
        closeOnSelect: false
    }
});

sentButton.addEventListener('click', function () {
    tags = [];
    let val = input.value;
    extractTagsFromJson(val, tags);
    tagsArea.value = Array.from(new Set(tags)).toString();
});

function extractTagsFromJson(val, tags) {
    if (val !== '') {
        let t = JSON.parse(val);
        t.forEach(e => tags.push(e.value));
    }
}