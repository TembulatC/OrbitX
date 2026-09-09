# Сборка (Node.js)
FROM node:20-alpine AS builder
WORKDIR /app

# Ставим зависимости (используем кэш Docker)
COPY orbitx.frontend/package*.json ./
RUN npm install --legacy-peer-deps

# Копируем исходники и собираем продакшен-версию (появится папка dist)
COPY orbitx.frontend/ ./
RUN npm run build

# Работа (Nginx)
FROM nginx:stable-alpine

# Копируем собранный фронтенд из первого этапа в папку Nginx
COPY --from=builder /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]