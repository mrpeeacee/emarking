# Stage 1 - Build Angular app
FROM node:18 AS build

WORKDIR /app

COPY . .

RUN npm install && npm run build

# The Angular build artifacts will be in /app/dist/
