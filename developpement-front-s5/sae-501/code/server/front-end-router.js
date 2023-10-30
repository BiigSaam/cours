import express from "express";
import path from "path";
import axios from "axios";
import fs from "fs/promises";

// Models
import SAE from "#models/sae.js";
import Article from "#models/article.js";

const router = express.Router();

router.use(async (_req, res, next) => {
    const originalRender = res.render;
    res.render = async function (view, local, callback) {
        const manifest = {
            manifest: await parseManifest(),
        };

        const args = [view, { ...local, ...manifest }, callback];
        originalRender.apply(this, args);
    };

    next();
});

const parseManifest = async () => {
    if (process.env.NODE_ENV !== "production") {
        return {};
    }

    const manifestPath = path.join(
        path.resolve(),
        "dist",
        "frontend.manifest.json"
    );
    const manifestFile = await fs.readFile(manifestPath);

    return JSON.parse(manifestFile);
};

router.get("/", async (req, res) => {
    const queryParams = new URLSearchParams(req.query).toString();
    let options = {
        method: "GET",
        url: `${res.locals.base_url}/api/articles?${queryParams}`,
    };
    let result = null;
    try {
        result = await axios(options);
    } catch (e) {}

    res.render("pages/front-end/index.twig", {
        list_articles: result.data,
    });
});

// "(.html)?" makes ".html" optional
router.get("/a-propos(.html)?", async (_req, res) => {
    let options = {
        method: "GET",
        url: `${res.locals.base_url}/api/saes?per_page=10`,
    };

    let result = null;
    try {
        result = await axios(options);
    } catch (e) {}

    res.render("pages/front-end/a-propos.twig", {
        list_saes: result.data,
    });
});

export default router;
