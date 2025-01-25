/** @type {import('tailwindcss').Config} */
export default {
  content: [
      "./index.html",
      "./src/**/*.{js,ts,jsx,tsx}"
  ],
  theme: {
    extend: {
      dropShadow: {
        glow: [
        "0 0px 5px rgba(0, 255, 0, 1)",
        "0 0px 10px rgba(50, 50, 50, 1)"
        ]
        }
    },
  },
  plugins: [],
}
