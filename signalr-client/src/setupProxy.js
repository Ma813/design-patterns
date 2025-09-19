const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function (app) {
    app.use(
        '/chatHub',
        createProxyMiddleware({
            target: 'https://localhost:7000',
            changeOrigin: true,
            ws: true,
            secure: false // This ignores SSL certificate errors
        })
    );
};