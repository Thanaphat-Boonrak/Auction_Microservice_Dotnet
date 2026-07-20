FROM --platform=$BUILDPLATFORM node:22-alpine AS builder

ARG NEXT_PUBLIC_NOTI_URL

ENV NEXT_PUBLIC_NOTI_URL=$NEXT_PUBLIC_NOTI_URL

WORKDIR /app/frontend/web-app

COPY frontend/web-app/package*.json ./

RUN npm ci --prefer-offline --no-audit

COPY frontend/web-app ./

RUN npm run build


FROM node:22-alpine AS runner

WORKDIR /app

ENV NODE_ENV=production

COPY --from=builder /app/frontend/web-app/public ./public
COPY --from=builder /app/frontend/web-app/.next/standalone ./frontend/web-app/
COPY --from=builder /app/frontend/web-app/.next/static ./frontend/web-app/.next/static

EXPOSE 3000

CMD ["node", "frontend/web-app/server.js"]