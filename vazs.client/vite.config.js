import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/Login": {
        target: "http://localhost:7251/Account",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
