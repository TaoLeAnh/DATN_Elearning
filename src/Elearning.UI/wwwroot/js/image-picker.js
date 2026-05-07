// Mở popup kích thước cố định (centered)
export function openPicker(url, w, h) {
    const dualScreenLeft = window.screenLeft !== undefined ? window.screenLeft : window.screenX;
    const dualScreenTop = window.screenTop !== undefined ? window.screenTop : window.screenY;

    const width = window.innerWidth || document.documentElement.clientWidth || screen.width;
    const height = window.innerHeight || document.documentElement.clientHeight || screen.height;

    const systemZoom = width / window.screen.availWidth;
    const left = (width - w) / (2 * systemZoom) + dualScreenLeft;
    const top = (height - h) / (2 * systemZoom) + dualScreenTop;

    const features = [
        `scrollbars=yes`,
        `resizable=yes`,
        `width=${w}`, `height=${h}`,
        `top=${top}`, `left=${left}`
    ].join(',');

    const win = window.open(url, '_blank', features);
    if (!win) {
        throw new Error('Trình duyệt đã chặn popup. Hãy cho phép popup cho trang này.');
    }
    win.focus();
    return true;
}

const handlers = new Map();

function tryParseOrWrap(raw) {
    try {
        return JSON.parse(raw);
    } catch {
        return { channel: null, payload: raw };
    }
}

export function registerMessageHandler(dotNetRef, allowedOrigins, channel) {
    const handler = async (event) => {
        if (Array.isArray(allowedOrigins) && allowedOrigins.length > 0) {
            if (!allowedOrigins.includes(event.origin)) return;
        }
        console.log(event);
        let raw = event.data;
        let data = (typeof raw === 'string') ? tryParseOrWrap(raw) : raw;
        if (data.channel !== channel) return;

        if (data.items) {
            await dotNetRef.invokeMethodAsync('OnMessageFromPicker', data.items);
        }
    };

    handlers.set(channel, handler);
    window.addEventListener('message', handler);
}

export function unregisterMessageHandler(channel) {
    const handler = handlers.get(channel);
    if (handler) {
        window.removeEventListener('message', handler);
        handlers.delete(channel);
    }
}

