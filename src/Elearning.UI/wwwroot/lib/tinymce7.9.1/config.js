(function () {
    const CNamespace = {
        picker: {
            baseUrl: "",
            mapKind: {
                image: "image", media: "media", file: "file"
            },
            allowedOrigins: [],
            w: 1280, h: 750,
            token: ""
        },
        uploadConfig: {
            domain: '',
            token: ''
        },
        setUploadConfig: function (domain, token) {
            if (domain) {
                CNamespace.uploadConfig.domain = domain;
                CNamespace.uploadConfig.token = token;
                console.log("Cấu hình upload cho TinyMCE đã được thiết lập.");
            } else {
                console.error("Lỗi: Thiếu domain hoặc token khi thiết lập cấu hình upload cho TinyMCE.");
            }
        },

        setToken: function (token) {
            if (token) {
                CNamespace.picker.token = token;
            }
            else
                alert("Token không hợp lệ")
        },

        setPickerUrl: function (baseUrl) {
            CNamespace.picker.baseUrl = baseUrl + "/type/";
            CNamespace.picker.allowedOrigins.push(baseUrl);
        },

        buildPickerUrl(kind) {
            const k = (kind || "image").toLowerCase();
            const typeId = CNamespace.picker.mapKind[k] ?? 1;
            const channel = (crypto?.randomUUID?.() || Math.random().toString(36).slice(2));
            const u = new URL(`${CNamespace.picker.baseUrl}${typeId}`, window.location.href);
            if (CNamespace.picker.token) u.searchParams.set("si-token", CNamespace.picker.token);
            u.searchParams.set("channel", channel);
            return { url: u.toString(), channel };
        },

        openMediaPickerAndWait: function (kind = "image") {
            return new Promise((resolve, reject) => {
                const { w, h, allowedOrigins } = CNamespace.picker;
                const { url, channel } = CNamespace.buildPickerUrl(kind);
                const left = (window.screen.width - w) / 2;
                const top = (window.screen.height - h) / 2;
                const win = window.open(url, "_blank", `width=${w},height=${h},left=${left},top=${top},resizable,scrollbars`);
                if (!win) return reject("Popup bị chặn.");

                const handler = (event) => {
                    if (Array.isArray(allowedOrigins) && allowedOrigins.length > 0 && !allowedOrigins.includes(event.origin)) {
                        return;
                    }
                    try {
                        const raw = typeof event.data === "string" ? JSON.parse(event.data) : event.data;
                        if (!raw) throw new Error("Payload thiếu dữ liệu");
                        if (!raw?.channel || raw.channel !== channel) return;

                        window.removeEventListener("message", handler);
                        resolve(raw);
                    } catch (e) {
                        window.removeEventListener("message", handler);
                        reject(e.message || e);
                    }
                };
                window.addEventListener("message", handler);
            });
        },

        insertImageViaCallback: function (cb, payload) {
            cb(payload.url, { alt: payload.alt || "", title: payload.publicKey || "" });
        },

        insertMediaIntoDialog: function (cb, payload) {
            const isAudio = (payload.contentType || "").startsWith("audio/");
            const poster = payload.poster || payload.thumbUrl || "";
            const directMediaUrl = payload.url || payload.playerUrl;

            if (!directMediaUrl) {
                console.error('Video/Audio URL is missing!');
                return;
            }

            const width = payload.width || "640";
            const height = payload.height || "360";

            const html = isAudio
                ? `<audio controls style="width:100%;" src="${directMediaUrl}"></audio>`
                : `<div style="position: relative; width: 100%; padding-bottom: 56.25%; margin: 1em 0;">
               <video src="${directMediaUrl}" ${poster ? `poster="${poster}"` : ""} 
                   controls playsinline 
                   style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover;">
               </video>
           </div>`;

            // Truyền đủ width/height để TinyMCE dialog nhận
            cb(directMediaUrl, {
                embed: html,
                poster,
                width: width,    // ← nhận từ payload nếu có
                height: height
            });
        },
        insertFileIntoDialog: function (cb, payload) {
            const url = payload.playerUrl || payload.url;
            const name = payload.fileName || payload.publicKey || (url.split('/').pop() || "tập tin");
            cb(url, { text: name, title: name });
        },

        editorInstance: null,

        config: {
            init_instance_callback: (editor) => {
                console.log('init_instance_callback invoked though JsConfSrc', editor);
            },
            content_style: `
        body { 
            font-family: 'Noto Sans', sans-serif;
            font-size: 14pt;
            line-height:1.4;
            text-align: justify;
            padding: 10px;
        }
        p {
            line-height: 1.4;
            text-align: justify;
        }
        img.centered {
            display: block !important;
            margin-left: auto !important;
            margin-right: auto !important;
        }

        p.img-caption {
            text-align: center !important;
            font-style: italic;
            color: #777;
            font-size: 0.9em;
            margin-top: 4px;
        }
    `,
            font_family_formats:
                'Noto Sans=Noto Sans, sans-serif;' +
                'Arial=arial, helvetica, sans-serif;' +
                'Georgia=georgia, palatino, serif;' +
                'Times New Roman=times new roman, times, serif;' +
                'Verdana=verdana, geneva, sans-serif;' +
                'Tahoma=tahoma, arial, helvetica, sans-serif;' +
                'Trebuchet MS=trebuchet ms, geneva, sans-serif;' +
                'Helvetica=helvetica, arial, sans-serif',
            branding: false,
            promotion: false,
            license_key: 'gpl',
            language: 'vi',
            //plugins: 'preview importcss searchreplace autolink directionality code  visualchars fullscreen image link media codesample table charmap nonbreaking anchor insertdatetime advlist lists wordcount help hr',
            plugins: 'preview importcss searchreplace autolink directionality code visualchars fullscreen image link media codesample table charmap nonbreaking anchor insertdatetime advlist lists wordcount help',
            external_plugins: {
                'tiny_mce_wiris': 'https://cdn.jsdelivr.net/npm/@wiris/mathtype-tinymce6/plugin.min.js'
            },
            menubar: 'edit view insert format table',
            //toolbar:
            //    "undo redo fullscreen| fontfamily fontsize | " +
            //    "bold italic underline strikethrough | forecolor backcolor | " +
            //    "alignleft aligncenter alignright alignjustify | lineheight | " +
            //    "link image insertmultipleimages media table hr boxtemplate| " +
            //    "bullist numlist outdent indent | " +
            //    "removeformat spellcheck_vi insertbr | code preview blocks",
            toolbar:
                "undo redo fullscreen| fontfamily fontsize | " +
                "bold italic underline strikethrough | forecolor backcolor | " +
                "alignleft aligncenter alignright alignjustify | lineheight | " +
                "link image insertmultipleimages media table hr boxtemplate| " +
                "bullist numlist outdent indent | " +
                "removeformat spellcheck_vi insertbr | code preview blocks | " +
                "tiny_mce_wiris_formulaEditor",
            toolbar_mode: 'wrap',
            paste_data_images: true,

            paste_as_text: false,
            paste_block_drop: true,            // cấm kéo-thả để khỏi lọt HTML bẩn
            smart_paste: false,                // không tự autolink

            // Loại style rác
            paste_remove_styles: true,
            paste_remove_spans: true,
            paste_strip_class_attributes: 'all',
            paste_remove_styles_if_webkit: true,
            paste_webkit_styles: 'none',

            // Giữ nội dung chính, bỏ inline style/class
            paste_preprocess: (editor, args) => {
                args.content = args.content
                    // chỉ bỏ style & class ở các thẻ khác, KHÔNG bỏ của iframe
                    .replace(/<(?!iframe)(\w+)([^>]*)\sstyle="[^"]*"([^>]*)>/gi, '<$1$2$3>')
                    .replace(/<(?!iframe)(\w+)([^>]*)\sclass="[^"]*"([^>]*)>/gi, '<$1$2$3>')
                    .replace(/\s(dir|align|border|cellpadding|cellspacing)="[^"]*"/gi, '');
            },
            valid_elements:
                '@[class|style],' +
                'p[class|style],' +
                'br,' +
                // Heading - khối đoạn văn
                'h1[class|style],h2[class|style],h3[class|style],' +
                'h4[class|style],h5[class|style],h6[class|style],' +
                // Inline format
                'strong/b[class|style],' +
                'em/i[class|style],' +
                'u[class|style],' +
                's[class|style],strike[class|style],' +
                'span[class|style],' +
                // Danh sách - thêm style để giữ list-style-type
                'ul[class|style],' +
                'ol[class|style],' +
                'li[class|style],' +
                // Link
                'a[href|title|target|rel|class|style],' +
                // Media
                'img[src|alt|title|class|style|width|height],' +
                'div[style|class],' +
                'hr[class|style],' +
                'video[src|poster|controls|playsinline|style|preload|width|height],' +
                'iframe[src|width|height|frameborder|allowfullscreen|style|class],' +
                'audio[src|controls|style|preload],' +
                'source[src|type|data-mce-fragment],' +
                // Table
                'table[border|cellpadding|cellspacing|width|height|class|style],' +
                'thead[class|style],tbody[class|style],tfoot[class|style],' +
                'tr[rowspan|class|style|border],' +
                'td[colspan|rowspan|class|style|width|height|border],' +
                'th[colspan|rowspan|class|style|width|height|border]',
            extended_valid_elements:
                'hr[class|style],' +
                'video[src|poster|controls|playsinline|style|preload|width|height],' +
                'audio[src|controls|style|preload],' +
                'iframe[src|width|height|frameborder|allowfullscreen|style|class],' +
                'source[src|type|data-mce-fragment],' +
                'table[border|cellpadding|cellspacing|width|height|class|style],' +
                'thead,tbody,tfoot,' +
                'tr[rowspan|class|style|border],' +
                'td[colspan|rowspan|class|style|width|height|border],' +
                'th[colspan|rowspan|class|style|width|height|border],' +
                'math[*],mfrac[*],msup[*],msub[*],mi[*],mo[*],mn[*],mrow[*],msqrt[*],mtext[*],mspace[*],mtable[*],mtr[*],mtd[*]',
            invalid_elements: '',
            forced_root_block: 'p',

            file_picker_types: 'image media file',
            file_picker_callback: (cb, value, meta) => {
                const kind = meta?.filetype === 'media' ? 'media' : (meta?.filetype === 'file' ? 'file' : 'image');
                CNamespace.openMediaPickerAndWait(kind)
                    .then(payload => {
                        if (!payload?.items || payload.items.length === 0) return;
                        payload.items.forEach(item => {
                            if (kind === 'image') {
                                if (CNamespace.editorInstance) {
                                    const alt = item.alt || "";
                                    const title = item.publicKey || "";
                                    const caption = item.alt || item.fileName || "";
                                    CNamespace.editorInstance.insertContent(
                                        `<p style="text-align: center;">
                                            <img class='centered' src="${item.url}" alt="${alt}" title="${title}">
                                            <p class='img-caption'>Nhập mô tả ảnh tại đây...</p>
                                        </p>`
                                    );
                                    tinymce.activeEditor.windowManager.close();
                                }
                            
                            } else if (kind === 'media') {
                                CNamespace.insertMediaIntoDialog(cb, item);
                            } else {
                                CNamespace.insertFileIntoDialog(cb, item);
                            }
                        });
                    })
                    .catch(err => console.warn("Media picker error:", err));
            },

            //images_upload_handler: async (blobInfo) => {
            //    if (!CNamespace.uploadConfig.domain || !CNamespace.uploadConfig.token) {
            //        throw new Error('Cấu hình upload chưa được thiết lập từ server.');
            //    }
            //    const uploadUrl = `${CNamespace.uploadConfig.domain}/api/v1/intergration/upload-image-for-paste`;

            //    try {
            //        const formData = new FormData();
            //        formData.append('file', blobInfo.blob(), blobInfo.filename());

            //        const res = await fetch(uploadUrl, {
            //            method: 'POST',
            //            body: formData,
            //            headers: {
            //                'si-token': CNamespace.uploadConfig.token
            //            }
            //        });

            //        if (!res.ok) throw new Error(`Lỗi HTTP! Trạng thái: ${res.status}`);

            //        const imageUrl = await res.text();
            //        if (!imageUrl) throw new Error('Server trả về một URL trống.');

            //        return imageUrl;
            //    } catch (error) {
            //        console.error('Lỗi khi tải ảnh lên:', error);
            //        throw new Error('Tải ảnh lên thất bại: ' + error.message);
            //    }
            //},
            images_upload_handler: async (blobInfo) => {
                const uploadUrl = `${CNamespace.uploadConfig.domain}/api/upload/image`;

                const formData = new FormData();
                formData.append('file', blobInfo.blob(), blobInfo.filename());

                const res = await fetch(uploadUrl, {
                    method: 'POST',
                    body: formData
                });

                if (!res.ok) throw new Error(`Lỗi HTTP: ${res.status}`);
                const imageUrl = await res.text();
                if (!imageUrl) throw new Error('Server trả về URL trống');
                return imageUrl;
            },
            setup: (editor) => {
                CNamespace.editorInstance = editor;
                editor.on('keydown', function (e) {
                    if (e.keyCode === 13) {
                        const node = editor.selection.getNode();

                        const captionNode = editor.dom.getParent(node, 'p.img-caption');

                        if (captionNode) {
                            e.preventDefault();

                            const newParagraph = editor.dom.create('p');

                            newParagraph.innerHTML = '&nbsp;';

                            editor.dom.insertAfter(newParagraph, captionNode);

                            editor.selection.setCursorLocation(newParagraph, 0);
                        }
                    }
                });
                //editor.on('PastePostProcess', async (e) => {


                //    if (!CNamespace.uploadConfig.domain || !CNamespace.uploadConfig.token) {
                //        console.error('Cấu hình upload chưa được thiết lập. Không thể upload ảnh được dán.');
                //        e.node.querySelectorAll('img[src^="data:"]').forEach(img => img.remove());
                //        return;
                //    }

                //    // ===== THÊM CLASS Căn giữ =====
                //    e.node.querySelectorAll('img').forEach(img => {
                //        // 1. Thêm class căn giữa (giữ nguyên logic cũ)
                //        if (!img.classList.contains('centered')) {
                //            img.classList.add('centered');
                //        }

                //        // 2. LOGIC MỚI: TÌM CAPTION THÔNG MINH HƠN
                //        let potentialCaptionNode = null;
                //        let insertionTarget = img; // Mặc định chèn caption sau img

                //        // Helper: Kiểm tra xem một node có phải là caption hợp lệ không
                //        const isCaptionCandidate = (node) => {
                //            return node &&
                //                node.innerText &&
                //                node.innerText.trim().length > 0 &&
                //                node.innerText.trim().length < 500 && // Giới hạn độ dài (tránh lấy nhầm cả bài văn)
                //                !/^H[1-6]$/.test(node.tagName); // Không lấy thẻ tiêu đề làm caption
                //        };

                //        // Case A: Ảnh trần (img) + Text (img.next)
                //        if (isCaptionCandidate(img.nextElementSibling)) {
                //            potentialCaptionNode = img.nextElementSibling;
                //        }
                //        // Case B: Ảnh trong thẻ A (a > img) -> Text nằm sau A
                //        else if (img.parentElement.tagName === 'A') {
                //            insertionTarget = img.parentElement; // Chuyển điểm chèn ra sau thẻ A
                //            if (isCaptionCandidate(img.parentElement.nextElementSibling)) {
                //                potentialCaptionNode = img.parentElement.nextElementSibling;
                //            }
                //            // Case C: Ảnh trong Div > A (div > a > img) -> Text nằm sau Div bao (thường gặp ở Moj, Dân trí...)
                //            else if (img.parentElement.parentElement.tagName === 'DIV' &&
                //                isCaptionCandidate(img.parentElement.parentElement.nextElementSibling)) {
                //                // Chuyển điểm chèn ra sau cái Div bao ngoài cùng
                //                insertionTarget = img.parentElement.parentElement;
                //                potentialCaptionNode = img.parentElement.parentElement.nextElementSibling;
                //            }
                //        }
                //        // Case D: Ảnh trong Div (div > img) -> Text nằm sau Div
                //        else if (img.parentElement.tagName === 'DIV') {
                //            insertionTarget = img.parentElement;
                //            if (isCaptionCandidate(img.parentElement.nextElementSibling)) {
                //                potentialCaptionNode = img.parentElement.nextElementSibling;
                //            }
                //        }

                //        // 3. XỬ LÝ NỘI DUNG CAPTION
                //        let captionHtml = 'Nhập mô tả ảnh tại đây...';
                //        let isDefault = true;

                //        if (potentialCaptionNode) {
                //            // Lấy nội dung HTML để giữ định dạng (in nghiêng, đậm...) của nguồn
                //            captionHtml = potentialCaptionNode.innerHTML;
                //            isDefault = false;
                //            // Xóa node gốc đi để tránh bị lặp nội dung
                //            potentialCaptionNode.remove();
                //        }

                //        // 4. TẠO THẺ CAPTION CHUẨN CỦA EDITOR
                //        // Kiểm tra xem tại vị trí chèn đã có caption chưa (để tránh chèn đè hoặc chèn kép)
                //        const nextNode = insertionTarget.nextElementSibling;
                //        if (!nextNode || !nextNode.classList.contains('img-caption')) {
                //            const desc = document.createElement('p');
                //            desc.innerHTML = captionHtml;
                //            desc.classList.add('img-caption');

                //            insertionTarget.insertAdjacentElement('afterend', desc);
                //        }
                //    });

                //    const imgs = e.node.querySelectorAll('img[src^="data:"]');
                //    if (imgs.length === 0) return;
                //    const uploadUrl = `${CNamespace.uploadConfig.domain}/api/v1/intergration/upload-image-for-paste`;

                //    for (const img of imgs) {
                //        try {
                //            const blob = await (await fetch(img.src)).blob();
                //            const formData = new FormData();
                //            formData.append('file', blob, 'pasted-image.png');

                //            const res = await fetch(uploadUrl, {
                //                method: 'POST',
                //                body: formData,
                //                headers: {
                //                    'si-token': CNamespace.uploadConfig.token
                //                }
                //            });

                //            if (!res.ok) throw new Error(`Lỗi HTTP! Trạng thái: ${res.status}`);

                //            const imageUrl = await res.text();
                //            if (!imageUrl) throw new Error('Server đã trả về một URL rỗng.');

                //            img.src = imageUrl;
                //        } catch (error) {
                //            console.error('Lỗi khi tải ảnh được dán:', error);
                //            img.remove();
                //        }
                //    }
                //});
                editor.on('PastePostProcess', async (e) => {

                    // Xóa ảnh file:/// từ Word
                    e.node.querySelectorAll('img[src^="file:"]').forEach(img => img.remove());

                    // Giữ nguyên phần caption logic ...

                    // Sửa phần upload ảnh base64
                    const imgs = e.node.querySelectorAll('img[src^="data:"]');
                    if (imgs.length === 0) return;

                    const uploadUrl = `${CNamespace.uploadConfig.domain}/api/upload/image`;

                    for (const img of imgs) {
                        try {
                            const blob = await (await fetch(img.src)).blob();
                            const formData = new FormData();
                            formData.append('file', blob, 'pasted-image.png');

                            const res = await fetch(uploadUrl, {
                                method: 'POST',
                                body: formData
                                // Bỏ header si-token
                            });

                            if (!res.ok) throw new Error(`Lỗi HTTP: ${res.status}`);
                            img.src = await res.text();
                        } catch (error) {
                            console.error('Lỗi upload ảnh:', error);
                            img.remove();
                        }
                    }
                });

                // Giữ nguyên logic của menu media2025
                editor.ui.registry.addMenuButton('media2025', {
                    text: 'Chèn từ Media2025',
                    fetch: (callback) => {
                        callback([
                            {
                                type: 'menuitem', text: 'Ảnh…', onAction: () => {
                                    CNamespace.openMediaPickerAndWait('image')
                                        .then(payload => {
                                            if (!payload?.items || payload.items.length === 0) return;
                                            payload.items.forEach(item => {
                                                const alt = item.alt || "";
                                                const title = item.publicKey || "";
                                                const caption = item.alt || item.fileName || "";
                                                let captionHtml = caption ? `<em style="display: block; font-size: 0.9em; color: #555; font-style: italic; margin-top: 4px;">${caption}</em>` : "";
                                                editor.insertContent(
                                                    `<p style="text-align: center;">
                                                        <img src="${item.url}" alt="${alt}" title="${title}">
                                                        ${captionHtml}
                                                    </p>`
                                                );
                                            });
                                        })
                                        .catch(e => console.warn(e));
                                }
                            },
                            {
                                type: 'menuitem', text: 'Video/Audio…', onAction: () => {
                                    CNamespace.openMediaPickerAndWait('media')
                                        .then(p => CNamespace.insertMedia(editor, p))
                                        .catch(e => console.warn(e));
                                }
                            },
                            {
                                type: 'menuitem', text: 'Tập tin…', onAction: () => {
                                    CNamespace.openMediaPickerAndWait('file')
                                        .then(p => CNamespace.insertFileLink(editor, p))
                                        .catch(e => console.warn(e));
                                }
                            }
                        ]);
                    }
                });

                // ===== THÊM NÚT KIỂM TRA CHÍNH TẢ TIẾNG VIỆT =====
                editor.ui.registry.addButton('spellcheck_vi', {
                    icon: 'spell-check',
                    tooltip: 'Kiểm tra chính tả tiếng Việt (LanguageTool)',
                    onAction: async function () {
                        const content = editor.getContent({ format: 'text' });

                        if (!content || content.trim() === '') {
                            editor.windowManager.open({
                                title: 'Kiểm tra chính tả',
                                body: {
                                    type: 'panel',
                                    items: [{
                                        type: 'htmlpanel',
                                        html: '<p>Không có nội dung để kiểm tra!</p>'
                                    }]
                                },
                                buttons: [{ type: 'cancel', text: 'Đóng' }]
                            });
                            return;
                        }

                        // Kiểm tra giới hạn
                        const textToCheck = content.substring(0, 20000).trim();

                        if (!textToCheck) {
                            alert('Nội dung rỗng!');
                            return;
                        }

                        editor.setProgressState(true);

                        try {
                            // ===== CÁCH ĐÚNG: Dùng URLSearchParams + toString() =====
                            const params = new URLSearchParams();
                            params.append('text', textToCheck);
                            params.append('language', 'vi');

                            const response = await fetch('https://api.languagetool.org/v2/check', {
                                method: 'POST',
                                headers: {
                                    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
                                },
                                body: params.toString()  // ⭐ QUAN TRỌNG: .toString()
                            });

                            if (!response.ok) {
                                const errorText = await response.text();
                                throw new Error(`HTTP ${response.status}: ${errorText}`);
                            }

                            const data = await response.json();
                            editor.setProgressState(false);

                            // Không có lỗi
                            if (!data.matches || data.matches.length === 0) {
                                editor.windowManager.open({
                                    title: 'Kết quả kiểm tra',
                                    body: {
                                        type: 'panel',
                                        items: [{
                                            type: 'htmlpanel',
                                            html: `
                                <div style="padding: 20px; text-align: center;">
                                    <div style="font-size: 48px; color: #4CAF50;">✓</div>
                                    <p style="color: #4CAF50; font-weight: bold; ">
                                        Không tìm thấy lỗi chính tả!
                                    </p>
                                    <p style="color: #666; ">
                                        Đã kiểm tra ${textToCheck.length.toLocaleString()} ký tự
                                    </p>
                                </div>
                            `
                                        }]
                                    },
                                    buttons: [{ type: 'cancel', text: 'Đóng' }]
                                });
                                return;
                            }

                            // Có lỗi - Hiển thị
                            let errorHtml = `
                <div style="padding: 10px;">
                    <div style="background: #f44336; color: white; padding: 10px; margin-bottom: 15px; border-radius: 4px;">
                        <strong>⚠️ Tìm thấy ${data.matches.length} lỗi</strong>
                    </div>
                    <div style="max-height: 450px; overflow-y: auto;">
                        <ul style="list-style: none; padding: 0; margin: 0;">
            `;

                            const maxDisplay = Math.min(data.matches.length, 15);

                            for (let i = 0; i < maxDisplay; i++) {
                                const match = data.matches[i];
                                const contextText = match.context.text;
                                const contextOffset = match.context.offset;
                                const contextLength = match.context.length;

                                const before = contextText.substring(0, contextOffset);
                                const errorText = contextText.substring(contextOffset, contextOffset + contextLength);
                                const after = contextText.substring(contextOffset + contextLength);

                                const suggestions = match.replacements
                                    .slice(0, 3)
                                    .map(r => r.value)
                                    .join(', ') || 'không có gợi ý';

                                const ruleCategory = match.rule?.category?.name || 'Lỗi chung';

                                errorHtml += `
                    <li style="margin-bottom: 15px; padding: 12px; background: #f9f9f9; border-left: 4px solid #f44336; border-radius: 4px;">
                        <div style="display: flex; justify-content: space-between; margin-bottom: 8px;">
                            <span style="background: #ff5722; color: white; padding: 2px 8px; border-radius: 3px; font-size: 11px; font-weight: bold;">
                                Lỗi ${i + 1}
                            </span>
                            <span style="background: #e0e0e0; padding: 2px 8px; border-radius: 3px; font-size: 11px;">
                                ${ruleCategory}
                            </span>
                        </div>
                        <div style="margin-bottom: 8px;">
                            <strong style="color: #333;">${match.message}</strong>
                        </div>
                        <div style="background: white; padding: 8px; border-radius: 3px; margin-bottom: 8px; font-family: 'Courier New', monospace; font-size: 13px; line-height: 1.5;">
                            <span style="color: #666;">"${before}</span><span style="background: #ffcdd2; color: #c62828; font-weight: bold; padding: 2px 4px; border-radius: 2px;">${errorText}</span><span style="color: #666;">${after}"</span>
                        </div>
                        <div style="color: #1976d2; ">
                            💡 <strong>Gợi ý:</strong> ${suggestions}
                        </div>
                    </li>
                `;
                            }

                            if (data.matches.length > 15) {
                                errorHtml += `
                    <li style="padding: 10px; text-align: center; color: #666; font-style: italic; background: #fff3cd; border-radius: 4px;">
                        ... và ${data.matches.length - 15} lỗi khác
                    </li>
                `;
                            }

                            errorHtml += '</ul></div></div>';

                            editor.windowManager.open({
                                title: 'Kết quả kiểm tra chính tả',
                                body: {
                                    type: 'panel',
                                    items: [{ type: 'htmlpanel', html: errorHtml }]
                                },
                                buttons: [{ type: 'cancel', text: 'Đóng' }],
                                size: 'large'
                            });

                        } catch (error) {
                            editor.setProgressState(false);

                            console.error('LanguageTool API Error:', error);

                            let errorMessage = 'Không thể kết nối đến LanguageTool API';
                            if (error.message.includes('429')) {
                                errorMessage = '⏱️ Đã vượt quá giới hạn 20 request/phút. Vui lòng thử lại sau 1 phút.';
                            } else if (error.message.includes('503')) {
                                errorMessage = '🔧 Server đang bảo trì. Vui lòng thử lại sau.';
                            } else if (error.message.includes('400')) {
                                errorMessage = '❌ Yêu cầu không hợp lệ: ' + error.message;
                            }

                            editor.windowManager.open({
                                title: 'Lỗi kết nối',
                                body: {
                                    type: 'panel',
                                    items: [{
                                        type: 'htmlpanel',
                                        html: `
                            <div style="padding: 20px;">
                                <p style="color: #f44336; font-weight: bold; ">${errorMessage}</p>
                                <details style="margin-top: 15px; cursor: pointer;">
                                    <summary style="color: #666;">Xem chi tiết lỗi</summary>
                                    <pre style="background: #f5f5f5; padding: 10px; margin-top: 10px; font-size: 12px; overflow: auto; border-radius: 4px; border: 1px solid #ddd;">${error.message}</pre>
                                </details>
                            </div>
                        `
                                    }]
                                },
                                buttons: [{ type: 'cancel', text: 'Đóng' }]
                            });
                        }
                    }
                });
                editor.ui.registry.addButton('insertbr', {
                    text: 'BR',
                    tooltip: 'Xuống dòng (Chèn <br>)',
                    onAction: function () {
                        editor.insertContent('<br>');
                    }
                });

                //đăng ký nút chọn nhiều ảnh
                editor.ui.registry.addButton('insertmultipleimages', {
                    icon: 'gallery',
                    tooltip: 'Chèn nhiều ảnh',
                    onAction: function () {
                        const { w, h, allowedOrigins } = CNamespace.picker;
                        const { url, channel } = CNamespace.buildPickerUrl('image');

                        // Thêm &multiple=enable
                        const multiUrl = url + '&multiple=enable';

                        const left = (window.screen.width - w) / 2;
                        const top = (window.screen.height - h) / 2;
                        const win = window.open(multiUrl, '_blank', `width=${w},height=${h},left=${left},top=${top},resizable,scrollbars`);
                        if (!win) {
                            alert('Popup bị chặn. Hãy cho phép popup cho trang này.');
                            return;
                        }

                        const handler = (event) => {
                            if (Array.isArray(allowedOrigins) && allowedOrigins.length > 0 && !allowedOrigins.includes(event.origin)) return;

                            try {
                                const raw = typeof event.data === 'string' ? JSON.parse(event.data) : event.data;
                                if (!raw || !raw.channel || raw.channel !== channel) return;

                                window.removeEventListener('message', handler);

                                if (!raw.items || raw.items.length === 0) return;

                                // Xếp từng ảnh từ trên xuống dưới
                                let html = '';
                                raw.items.forEach(item => {
                                    const alt = item.alt || '';
                                    const title = item.publicKey || '';
                                    html += `
<p style="text-align: center;">
    <img class="centered" src="${item.url}" alt="${alt}" title="${title}" style="max-width:100%;">
</p>
<p class="img-caption">Nhập mô tả ảnh tại đây...</p>
`;
                                });

                                editor.insertContent(html);

                            } catch (e) {
                                window.removeEventListener('message', handler);
                                console.warn('Insert multiple images error:', e);
                            }
                        };

                        window.addEventListener('message', handler);
                    }
                });
                // ===== TỰ ĐỘNG CĂN ĐỀU CHO TEXT =====
                editor.on('NodeChange', function (e) {
                    const node = editor.selection.getNode();
                    if (node.nodeName === 'P' && !node.querySelector('img')) {
                        if (!node.style.textAlign || node.style.textAlign === '') {
                            editor.dom.setStyle(node, 'text-align', 'justify');
                        }
                    }
                });

                // Định nghĩa 3 template box đơn giản

                const BOX_TEMPLATES = [
                    {
                        id: 'quote',
                        name: 'Trích dẫn',
                        displayName: '❝ Trích dẫn ❞',
                        icon: '❝',
                        iconBottom: '❞',
                        color: '#616161',
                        bgColor: '#fafafa',
                        borderColor: '#e0e0e0',
                        placeholder: 'Nhập nội dung trích dẫn tại đây...',
                        description: 'Box trích dẫn với dấu ngoặc kép lớn',
                        type: 'quote'
                    },

                    
                    {
                        id: 'tip',
                        name: 'Mẹo',
                        displayName: 'Mẹo',
                        icon: '💡',
                        color: '#f57c00',
                        bgColor: '#fff3e0',
                        borderColor: '#ffe0b2',
                        placeholder: 'Nhập mẹo hữu ích tại đây...',
                        description: 'Chia sẻ tips, tricks hữu ích',
                        type: 'icon-box'
                    },
                    {
                        id: 'note',
                        name: 'Ghi chú',
                        displayName: 'Ghi chú',
                        icon: '📌',
                        color: '#616161',
                        bgColor: '#fafafa',
                        borderColor: '#e0e0e0',
                        placeholder: 'Nhập ghi chú tại đây...',
                        description: 'Ghi chú, lưu ý quan trọng',
                        type: 'icon-box'
                    },
                    {
                        id: 'warning',
                        name: 'Cảnh báo',
                        displayName: 'Cảnh báo',
                        icon: '⚠️',
                        color: '#f57c00',
                        bgColor: '#fff3e0',
                        borderColor: '#ffe0b2',
                        placeholder: 'Nhập nội dung cảnh báo tại đây...',
                        description: 'Cảnh báo, lưu ý quan trọng',
                        type: 'icon-box'
                    }
                ];

                // Hàm tạo HTML cho box template
                function createBoxHTML(template, content = '') {
                    const finalContent = content.trim() || template.placeholder;

                    // Template TRÍCH DẪN (có dấu ngoặc kép lớn ❝ ❞)
                    if (template.type === 'quote') {
                        return `
<div class="box-template box-quote" style="
    background-color: ${template.bgColor};
    border: 2px solid ${template.borderColor};
    padding: 24px 30px;
    margin: 24px 0;
    border-radius: 8px;
    position: relative;
">
    <div style="
        font-size: 72px;
        line-height: 0.5;
        color: ${template.color};
        opacity: 0.25;
        margin-bottom: 20px;
    ">❝</div>
    
    <div style="
        color: #333;
        line-height: 1.8;
        font-style: italic;
        padding: 0 20px;
        margin: 15px 0;
    ">${finalContent}</div>
    
    <div style="
        font-size: 72px;
        line-height: 0.5;
        color: ${template.color};
        opacity: 0.25;
        text-align: right;
        margin-top: 20px;
    ">❞</div>
</div>
<p><br></p>
`.trim();
                    }

                    // Template ICON BOX (info, tip, note, warning, success)
                    return `
<div class="box-template box-${template.id}" style="
    background-color: ${template.bgColor};
    border: 2px solid ${template.borderColor};
    padding: 18px 22px;
    margin: 20px 0;
    border-radius: 8px;
    position: relative;
">
    <div style="display: flex; align-items: flex-start; gap: 14px;">
        <div style="
            font-size: 32px;
            line-height: 1;
            flex-shrink: 0;
            margin-top: 0px;
        ">${template.icon}</div>
        <div style="flex: 1; margin-top:0px;">
            <div style="
                font-weight: bold;
                color: ${template.color};
                margin-top: 0px;
                margin-bottom: 10px;
            ">${template.name}</div>
            <div style="
                color: #333;
                
                line-height: 1.7;
            ">${finalContent}</div>
        </div>
    </div>
</div>
<p><br></p>
`.trim();
                }

                // Hàm tạo HTML cho modal chọn template
                function createTemplateModalHTML() {
                    let html = `
<div style="padding: 16px;">
    <h3 style="margin: 0 0 20px 0; font-size: 20px; color: #333; font-weight: 600;">Chọn kiểu Box</h3>
    <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 16px; max-height: 520px; overflow-y: auto; padding: 4px;">
`;

                    BOX_TEMPLATES.forEach(template => {
                        // Preview riêng cho từng loại template
                        let previewHTML = '';

                        if (template.type === 'quote') {
                            // Preview cho Quote box
                            previewHTML = `
                <div style="
                    margin-top: 12px;
                    padding: 6px 6px;
                    background: white;
                    border-radius: 6px;
                    position: relative;
                    border: 1px solid ${template.borderColor};
                ">
                    <div style="font-size: 28px; color: ${template.color}; opacity: 0.3; line-height: 0.8;">❝</div>
                    <div style="
                        font-size: 11px;
                        color: #666;
                        font-style: italic;
                        padding: 8px 12px;
                        line-height: 1.5;
                    ">
                        ${template.placeholder.substring(0, 45)}...
                    </div>
                    <div style="font-size: 28px; color: ${template.color}; opacity: 0.3; line-height: 0.8; text-align: right;">❞</div>
                </div>
            `;
                        } else {
                            // Preview cho Icon box
                            previewHTML = `
                <div style="
                    margin-top: 12px;
                    padding: 12px;
                    background: white;
                    border-radius: 6px;
                    border: 1px solid ${template.borderColor};
                    display: flex;
                    align-items: flex-start;
                    gap: 10px;
                ">
                    <div style="font-size: 20px;">${template.icon}</div>
                    <div style="flex: 1;">
                        <div style="font-weight: bold; color: ${template.color}; font-size: 11px; margin-bottom: 4px;">${template.name}</div>
                        <div style="font-size: 10px; color: #666; line-height: 1.4;">
                            ${template.placeholder.substring(0, 40)}...
                        </div>
                    </div>
                </div>
            `;
                        }

                        html += `
        <div class="template-item" data-template-id="${template.id}" style="
            border: 2px solid ${template.borderColor};
            background: ${template.bgColor};
            padding: 18px;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.25s ease;
            position: relative;
        " 
        onmouseover="
            this.style.transform='translateY(-4px)'; 
            this.style.boxShadow='0 6px 20px rgba(0,0,0,0.12)';
            this.style.borderColor='${template.color}';
        "
        onmouseout="
            this.style.transform='translateY(0)'; 
            this.style.boxShadow='none';
            this.style.borderColor='${template.borderColor}';
        ">
            <div style="display: flex; align-items: center; gap: 12px; margin-bottom: 10px;">
                <span style="font-size: 28px; line-height: 1;">${template.icon}</span>
                <strong style="color: ${template.color};font-weight: 600;">${template.displayName}</strong>
            </div>
            <p style="margin: 0 0 8px 0; font-size: 12px; color: #666; line-height: 1.5;">
                ${template.description}
            </p>
            ${previewHTML}
        </div>
        `;
                    });

                    html += `
    </div>
    <div style="margin-top: 20px; padding-top: 16px; border-top: 1px solid #e0e0e0;">
        <p style="margin: 0; font-size: 12px; color: #999; text-align: center;">
            💡 Tip: Bôi đen text trước khi chọn để wrap nội dung vào box
        </p>
    </div>
</div>
`;

                    return html;
                }

                // ===== ĐĂNG KÝ NÚT TRONG TOOLBAR =====
                editor.ui.registry.addButton('boxtemplate', {
                    icon: 'comment-add',
                    tooltip: 'Chèn Box Template',
                    onAction: function () {
                        const modalHtml = createTemplateModalHTML();

                        const dialog = editor.windowManager.open({
                            title: 'Chọn Box Template',
                            body: {
                                type: 'panel',
                                items: [
                                    {
                                        type: 'htmlpanel',
                                        html: modalHtml
                                    }
                                ]
                            },
                            buttons: [
                                {
                                    type: 'cancel',
                                    text: 'Đóng'
                                }
                            ],
                            size: 'large'
                        });

                        // Thêm event listener cho các template items
                        setTimeout(() => {
                            const modalElement = document.querySelector('.tox-dialog');
                            if (modalElement) {
                                const templateItems = modalElement.querySelectorAll('.template-item');

                                templateItems.forEach(item => {
                                    item.addEventListener('click', function () {
                                        const templateId = this.getAttribute('data-template-id');
                                        const template = BOX_TEMPLATES.find(t => t.id === templateId);

                                        if (template) {
                                            // Lấy nội dung đang được select
                                            const selectedContent = editor.selection.getContent({ format: 'text' });

                                            // Tạo HTML box
                                            const boxHtml = createBoxHTML(template, selectedContent);

                                            // Chèn vào editor
                                            editor.insertContent(boxHtml);

                                            // Đóng modal
                                            dialog.close();
                                        }
                                    });
                                });
                            }
                        }, 100);
                    }
                });


                //kết thúc
            }
            ,

        }
    };

    window.CNamespace = CNamespace;
})();
