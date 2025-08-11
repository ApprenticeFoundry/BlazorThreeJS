#!/bin/sh
echo "Building Typescript from Javascript"
cd ./JsLib
npm run build
cd ..
echo "Done..."
dotnet build