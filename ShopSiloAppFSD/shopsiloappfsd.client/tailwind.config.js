/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./src/**/*.{js,jsx,ts,tsx}", // Adjust this if your file structure is different
    ],
    theme: {
        extend: {
            colors: {
                utorange: '#FF8811',
                lightjasmine: '#f3dea7',
                jasmine: '#F4D06F',
                darkjasmine: '#edc148',
                tiffanyblue: '#9DD9D2',
                floralwhite: '#FFF8F0',
                spacecadet: '#473485',
                mustard: '#E1AD01',
                darkmustard: '#C78C06',
            }
        },
    },
    plugins: [
        require('tailwind-scrollbar-hide')
    ],
};
