import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
        port: 63452,
        host: true, // Enable access from outside container
    },
    preview: {
        port: 63452,
        host: true,
    }
})
