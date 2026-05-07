(function () {
    function escapeHtml(s) {
        if (s === null || s === undefined) return "";
        return String(s)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    function buildTable(columns, rows) {

        const colgroup = columns
            .map(c => `<col style="width:${escapeHtml(c.width || "auto")}">`)
            .join("");

        const thead = `<tr>${columns.map(c => `<th style="text-align:${escapeHtml(c.align || "left")}">${escapeHtml(c.title || "")}</th>`).join("")
            }</tr>`;


        const tbody = (rows || []).map(r => {
            return `<tr>${columns.map(c => {
                const val = c.field ? r?.[c.field] : "";
                return `<td style="text-align:${escapeHtml(c.align || "left")}">${escapeHtml(val)}</td>`;
            }).join("")
                }</tr>`;
        }).join("");

        return `
      <table class="bordered">
        <colgroup>${colgroup}</colgroup>
        <thead>${thead}</thead>
        <tbody>${tbody}</tbody>
      </table>`;
    }

    // columns: [{ title, field, width, align }]
    // rows: array of plain objects: { field1: "...", field2: "..." }
    window.printTable = function (opts) {
        const title = opts?.title ?? "In ấn";
        const headerHtml = opts?.headerHtml ?? "";
        const footerHtml = opts?.footerHtml ?? "";
        const columns = opts?.columns ?? [];
        const rows = opts?.rows ?? [];
        const customCss = opts?.css ?? "";

        const w = window.open("", "", "height=800,width=1000");
        if (!w) return;

        const tableHtml = buildTable(columns, rows);

        w.document.write(`<!DOCTYPE html><html><head><meta charset="utf-8"><title>${escapeHtml(title)}</title>`);
        w.document.write(`<style>
      @page { size: A4; margin: 15mm 12mm; }
      body { font-family: "Times New Roman", serif; font-size: 13pt; }
      table { width: 100%; border-collapse: collapse; }
      .bordered th, .bordered td { border: 1px solid #000; padding: 6px; vertical-align: top; }
      th { background: #f0f0f0; font-weight: bold; }
      .print-header { margin-bottom: 10px; }
      .print-footer { margin-top: 14px; }
      ${customCss}
    </style></head><body>`);

        w.document.write(`<div class="print-header">${headerHtml}</div>`);
        w.document.write(tableHtml);
        w.document.write(`<div class="print-footer">${footerHtml}</div>`);

        w.document.write(`</body></html>`);
        w.document.close();
        w.focus();

        setTimeout(() => { w.print(); w.close(); }, 250);
    };

    window.printRawHtml = function (opts) {
        const title = opts?.title ?? "In ấn";
        const html = opts?.html ?? "";
        const css = opts?.css ?? "";

        const w = window.open("", "", "height=800,width=1000");
        if (!w) return;

        w.document.write(`<!DOCTYPE html><html><head><meta charset="utf-8"><title>${title}</title>`);
        w.document.write(`<style>
    @page { size: A4; margin: 15mm 12mm; }
    body { font-family: "Times New Roman", serif; font-size: 13pt; }
    table { width: 100%; border-collapse: collapse; }
    .bordered th, .bordered td { border: 1px solid #000; padding: 6px; vertical-align: top; }
    th { background: #f0f0f0; font-weight: bold; }
    ${css}
  </style></head><body>`);

        w.document.write(html);

        w.document.write(`</body></html>`);
        w.document.close();
        w.focus();
        setTimeout(() => { w.print(); w.close(); }, 250);
    };
})();
