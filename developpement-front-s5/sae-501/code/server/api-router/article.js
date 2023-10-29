import express from "express";
import fs from "fs";

import Article from '#models/article.js'
import { imageValidator } from  "#database/validator.js";

import upload from "../uploader.js"

const router = express.Router();

const base = "articles";

router.get(`/${base}`, async (req, res) => {
    const page = req.query.page || 1;
    let perPage = req.query.per_page || 7;
    perPage = Math.min(perPage, 20);

    const listRessources = await Article.find()
        .skip(Math.max(page - 1, 0) * perPage)
        .limit(perPage)
        .sort({ _id: -1 })
        .lean()
        .orFail()
        .catch(() => {
            return {};
        });

    const count = await Article.count();

    return res.status(200).json({
        data: listRessources,
        total_pages: Math.ceil(count / perPage),
        count,
        page,
    })
});

router.get(`/${base}/:id`, async (req, res) => {
    let listErrors =  []

    const ressource = await Article.findOne({ _id: req.params.id }).orFail().catch((err) => {
        res.status(404).json({errors: [...listErrors, "Élément non trouvé"]})
    });

    return res.status(200).json(ressource)
});

router.post(`/${base}`, upload.single("image"), async (req, res) => {
    let imagePayload = {}
    let listErrors =  []
    let targetPath = undefined;

    const uploadedImage = req.body.file || req.file;

    if (uploadedImage) {
        let imageName;
        ({image_path: targetPath, errors: listErrors, image_name: imageName} = uploadImage(uploadedImage, res.locals.upload_dir))
        imagePayload = { image: imageName }
    }

    if(listErrors.length) {
        return res.status(400).json({ 
            errors: listErrors, 
            ressource: req.body 
        })
    }

    let ressource = new Article({ ...req.body, ...imagePayload });

    await ressource.save({ validateBeforeSave: true }).then(() => {
        res.status(201).json(ressource)
    })
    .catch((err) => {
        res.status(400).json({
            errors: [
                ...listErrors, 
                ...deleteUpload(targetPath), 
                ...Object.values(err?.errors).map((val) => val.message)
            ]
        })
    })
});

router.put(`/${base}/:id`, upload.single("image"), async (req, res) => {
    let imagePayload = {}
    let listErrors =  []
    let targetPath = undefined;

    const uploadedImage = req.body.file || req.file;
    
    if (uploadedImage) {
        let imageName;
        ({image_path: targetPath, errors: listErrors, image_name: imageName} = uploadImage(uploadedImage, res.locals.upload_dir))
        imagePayload = { image: imageName }
    }

    let oldRessource = await Article.findById(req.params.id).lean();
    if (!oldRessource) {
        oldRessource = {}
    }

    if(listErrors.length) {
        return res.status(400).json({ 
            errors: listErrors, 
            ressource: { ...oldRessource, ...req.body }
        })
    }

    const ressource = await Article.findOneAndUpdate({ _id: req.params.id }, { ...req.body, _id: req.params.id, ...imagePayload }, { new: true })
    .orFail()
    .catch((err) => {
        // err instanceof mongoose.DocumentNotFoundError
        if(err instanceof mongoose.CastError) {
            res.status(400).json({errors: [...listErrors, "Élément non trouvé", ...deleteUpload(targetPath)]})
        } else {
            res.status(400).json({ 
                errors: [...listErrors, ...Object.values(err?.errors || []).map((val) => val.message), ...deleteUpload(targetPath), "Erreur"], 
                ressource: { ...oldRessource, ...req.body }
            })
        }
    });

    return res.status(201).json(ressource)
});

router.delete(`/${base}/:id`, async (req, res) => {
    const ressource = await Article.findByIdAndDelete(req.params.id).orFail().catch(() => {});

    if(!ressource) {
        return res.status(404).json({ error: "Quelque chose s'est mal passé, veuillez recommencer" })
    }
    
    if(ressource.image) {
        try {
            const targetPath = `${res.locals.upload_dir}${ressource.image}`
            await fs.unlink(targetPath)
        } catch (e) {
            // return res.status(404).json({ error: "L'image n'a pas pu être supprimée" })
        }
    }

    return res.status(200).json(ressource);
});

export default router;
