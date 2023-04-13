FROM node as builder

WORKDIR /app
COPY package.json yarn.lock ./
RUN yarn install

COPY . .
RUN yarn build

FROM node

WORKDIR /app
COPY package.json yarn.lock ./
RUN yarn install --production

COPY --from=builder /app/dist ./dist
CMD [ "yarn", "run", "start" ]