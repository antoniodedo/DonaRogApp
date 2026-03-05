FROM node:18-alpine AS build

WORKDIR /app
COPY package*.json /app/

RUN yarn
COPY . /app

RUN yarn build:prod

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN tr -d '\r' < /docker-entrypoint.sh > /tmp/ep.sh \
    && mv /tmp/ep.sh /docker-entrypoint.sh \
    && chmod +x /docker-entrypoint.sh

COPY --from=build /app/dist/TodoApp /usr/share/nginx/html

ENTRYPOINT ["/docker-entrypoint.sh"]
