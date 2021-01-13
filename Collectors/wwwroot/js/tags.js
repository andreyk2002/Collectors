let input = document.querySelector('input[name="input-custom-dropdown"]');
let tagsArea = document.getElementById('tags');
let tags = tagsArea.value.split(',');
let tagify = new Tagify(input, {
    whitelist: tags,
    blacklist: ["Something"],
    maxTags: 7,
    dropdown: {
        maxItems: 20,
        classname: "tags-look",
        enabled: 0,
        closeOnSelect: false
    }
});

input.addEventListener('change', onChange)


function onChange(e) {
    tags = [];
    if (e.target.value !== '') {
        let t = JSON.parse(e.target.value);
        t.forEach(e => tags.push(e.value));
    }
    tagsArea.value = Array.from(new Set(tags)).toString();
}