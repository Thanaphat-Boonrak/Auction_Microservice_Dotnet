FROM --platform=$BUILDPLATFORM node:22-alpine AS builder

WORKDIR /app/frontend/web-app

COPY frontend/web-app/package*.json ./


RUN npm ci --prefer-offline --no-audit

COPY . /app

RUN npm run build


FROM node:22-alpine AS runner

WORKDIR /app

ENV NODE_ENV=production

COPY --from=builder /app/frontend/web-app/public ./public
COPY --from=builder /app/frontend/web-app/.next/standalone ./
COPY --from=builder /app/frontend/web-app/.next/static ./.next/static

EXPOSE 3000

CMD ["node", "server.js"]