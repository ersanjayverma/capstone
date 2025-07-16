// wwwroot/js/fingerprint.js

window.getFingerprint = async () => {
    const fp = await FingerprintJS.load();
    const result = await fp.get();
    return result.visitorId;
};
 window.renderChartFromElement = (canvasElement, config) => {
        if (!window.myCharts) window.myCharts = {};

        if (!canvasElement) {
            console.warn("Canvas element not found.");
            return;
        }

        const ctx = canvasElement.getContext('2d');

        if (!ctx) {
            console.error("Canvas context not available.");
            return;
        }

        const id = canvasElement.id || Math.random().toString(36).substring(2);
        canvasElement.id = id;

        if (window.myCharts[id]) {
            window.myCharts[id].destroy();
        }

        window.myCharts[id] = new Chart(ctx, config);
    };