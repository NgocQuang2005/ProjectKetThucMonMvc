﻿//ImageFile.onchange = evt => {
//    const [file] = ImageFile.files
//    if (file) {
//        preview.src = URL.createObjectURL(file);
//    }
//}
ClassicEditor
    .create(document.querySelector('#editor'), {
        extraPlugins: [CustomUploadAdapterPlugin],
        toolbar: ['Essentials',
            'Paragraph',
            'Heading', '|',
            'Bold',
            'Italic',
            'Underline',
            'Strikethrough', '|',
            'SourceEditing',
            'Subscript',
            'Superscript',
            'Alignment',
            'Indent',
            'IndentBlock',
            'BlockQuote', '|',
            'Link',
            'Image',
            'ImageCaption',
            'ImageStyle',
            'ImageToolbar',
            'ImageUpload',
            'List',
            'MediaEmbed', '|',
            'PasteFromOffice',
            'insertTable',
            'Highlight',
            'FontFamily',
            'FontSize',
            'FontColor',
            'FontBackgroundColor', 'GeneralHtmlSupport', /* ... */]
        ,
        image: {
            // Cấu hình các kiểu hình ảnh
            styles: [
                'full',
                'side'
            ],
            toolbar: [
                'imageStyle:full',
                'imageStyle:side',
                '|',
                'imageTextAlternative'
            ]
        },
        // Cấu hình để tải hình ảnh lên server của bạn
        imageUpload: {
            uploadUrl: '/api/image/upload',
            headers: {
                'X-CSRF-TOKEN': 'CSRF-Token', // If CSRF token is needed
            }
        }
    })
    .then(editor => {
        const initialData = document.querySelector('#hiddenNcontent').value;
        editor.setData(initialData);
        editor.model.document.on('change:data', () => {
            document.querySelector('#hiddenNcontent').value = editor.getData();
        });
    })
    .catch(error => {
        console.error(error);
    });


function CustomUploadAdapterPlugin(editor) {
    editor.plugins.get('FileRepository').createUploadAdapter = (loader) => {
        return new CustomUploadAdapter(loader);
    };
}
class CustomUploadAdapter {
    constructor(loader) {
        this.loader = loader;
    }

    upload() {
        return this.loader.file
            .then(file => new Promise((resolve, reject) => {
                const data = new FormData();
                data.append('upload', file);

                fetch('/Admin/Events/UploadImage', {
                    method: 'POST',
                    body: data,
                })
                    .then(response => response.json())
                    .then(result => {
                        if (result.uploaded) {
                            resolve({
                                default: result.url // Adjust depending on the API response
                            });
                        } else {
                            reject(result.error.message);
                        }
                    })
                    .catch(error => {
                        reject('Upload failed');
                    });
            }));
    }

    abort() {
        // Handle aborting the upload process
    }
}