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

    // Ensure canvas internal resolution matches rendered size
    const rect = canvasElement.getBoundingClientRect();
    canvasElement.width = rect.width;
    canvasElement.height = rect.height;

    // Ensure the canvas has a stable ID
    const id = canvasElement.id || Math.random().toString(36).substring(2);
    canvasElement.id = id;

    // Destroy previous chart if it exists
    if (window.myCharts[id]) {
        window.myCharts[id].destroy();
    }

    // Create and store the new chart
    window.myCharts[id] = new Chart(ctx, {
        ...config,
        options: {
            ...config.options,
            responsive: true,
            maintainAspectRatio: false,
        }
    });
};
