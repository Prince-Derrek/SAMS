/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Views/**/*.cshtml",
        "./Pages/**/*.cshtml",
        "./wwwroot/**/*.html",
        "./node_modules/flowbite/**/*.js"
    ],
    theme: {
        extend: {
            colors: {
                primary: {
                    '50': '#f2f8fd',
                    '100': '#e3effb',
                    '200': '#c1e1f6',
                    '300': '#8bc8ee',
                    '400': '#4dabe3',
                    '500': '#2691d1',
                    '600': '#1773b2',
                    '700': '#16659e',
                    '800': '#154f77',
                    '900': '#174263',
                    '950': '#0f2b42',
                },
                secondary: {
                    '50': '#fff1f1',
                    '100': '#ffe0e0',
                    '200': '#ffc7c7',
                    '300': '#ffa0a0',
                    '400': '#ff6969',
                    '500': '#fa3939',
                    '600': '#e71b1b',
                    '700': '#c31212',
                    '800': '#a81414',
                    '900': '#851717',
                    '950': '#490606',
                },

            },
        },
    },
    plugins: [
        require("flowbite/plugin")
    ],
}