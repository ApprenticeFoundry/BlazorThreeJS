const path = require('path');

module.exports = {
    entry: './src/index.ts',
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                },
            },
            {
                test: /\.(ts|tsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'ts-loader',
                },
            },
            { test: /\.css$/, use: ['style-loader', 'css-loader'] },
            {
                test: /\.(png|jpe?g|gif)$/i,
                use: [
                    {
                        loader: 'file-loader',
                    },
                ],
            },
            {
                test: /\.(glb|gltf)$/i,
                use: [
                    {
                        loader: 'file-loader',
                        options: {
                            outputPath: 'assets/models/',
                        },
                    },
                ],
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.jsx', '.js', '.png', '.jpg', '.jpeg', '.glb', '.gltf'],
    },
    output: {
        path: path.resolve(__dirname, '../wwwroot/dist'),
        // Note: Renamed from 'app-lib.js' to prevent Blazor static web asset conflicts
        // See ASSET_NAMING_CHANGES.md for details
        filename: 'app-lib-threejs.js',
        library: 'AppLib',
        clean: true,
    },
};
