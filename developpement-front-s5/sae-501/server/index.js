import express from "express";
import path from "path";
import fs from "fs";
import lodash from "lodash";
import FastGlob from "fast-glob";
import livereload from "livereload";
import connectLiveReload from "connect-livereload";
import { fileURLToPath } from "url";
import dotenv from "dotenv";

import frontendRouter from "./front-end-router.js";

let envFilePath = '.env.prod.local';
if(process.env.NODE_ENV === "development") {
  envFilePath = '.env.dev.local';
}
const envVars = dotenv.config({ path: envFilePath })

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const liveReloadServer = livereload.createServer();
liveReloadServer.server.once("connection", () => {
  setTimeout(() => {
    liveReloadServer.refresh("/");
  }, 100);
});

const port = envVars?.parsed?.PORT || 3000;
const hostname = "127.0.0.1";
const app = express();

app.use(
  connectLiveReload({
    port: 35729,
  })
);

let jsonFilesContent = {};
FastGlob.sync("./src/data/**/*.json").forEach((entry) => {
  const filePath = path.resolve(entry);
  jsonFilesContent = lodash.merge(
    jsonFilesContent,
    JSON.parse(fs.readFileSync(filePath).toString())
  );
});

app.use(function (_req, res, next) {
  res.locals = {...jsonFilesContent, ...{
    NODE_ENV: process.env.NODE_ENV,
    ...envVars.parsed
  }};

  const originalRender = res.render;
  res.render = function (view, local, callback) {
    let tplContent = {};

    const tplContentPath = path.join(__dirname, "..", `/src/${path}.json`);
    if (fs.existsSync(tplContentPath)) {
      tplContent = JSON.parse(fs.readFileSync(tplContentPath).toString());
    }

    const args = [view, { ...local, ...tplContent }, callback];

    originalRender.apply(this, args);
  };

  next();
});

const publicPath = path.join(path.resolve(), "public");
app.use("/", express.static(publicPath));
app.set("view engine", "twig");
app.set("views", path.join(__dirname, "..", "/src"));

app.use(frontendRouter);

app.listen(port, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});
