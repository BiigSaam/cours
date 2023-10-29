import express from "express";
import mongoose from "mongoose";
import axios from "axios";
import fs from "fs";

// Models
import SAE from "#models/sae.js";

import { imageValidator } from "#database/validator.js";
import upload from "../uploader.js";

const base = "saes";
const router = express.Router();

router.get(`/${base}`, async (req, res) => {
    const page = req.query.page || 1;
    let perPage = req.query.per_page || 7;
    perPage = Math.min(perPage, 20);

    const listSAEs = await SAE.find()
        .skip(Math.max(page - 1, 0) * perPage)
        .limit(perPage)
        .sort({ _id: -1 })
        .lean()
        .orFail()
        .catch(() => {
            return {};
        });

    const count = await SAE.count();

    res.render("pages/back-end/saes/list.twig", {
        list_saes: {
            data: listSAEs,
            total_pages: Math.ceil(count / perPage),
            count,
            page,
        },
    });
});

router.get([`/${base}/:id`, `/${base}/add`], async (req, res) => {
    const sae = await SAE.findOne({ _id: req.params.id })
        .orFail()
        .catch(() => {
            return {};
        });

    res.render("pages/back-end/saes/add-edit.twig", {
        sae,
        is_edit: mongoose.Types.ObjectId.isValid(req.params.id),
    });
});

// https://stackoverflow.com/questions/15772394/how-to-upload-display-and-save-images-using-node-js-and-express

router.post(`/${base}/:id`, upload.single("image"), async (req, res) => {
    let ressource = null;
    const isEdit = mongoose.Types.ObjectId.isValid(req.params.id)
    let listErrors = [];
    let options = {
        headers: {
            "Content-Type": "multipart/form-data",
        },
        data: {
            ...req.body,
            file: req.file,
        },
    }

    if(isEdit) {
        options = {
            ...options,
            method: "PUT",
            url: `${res.locals.base_url}/api/saes/${req.params.id}`,
        }
    } else {
        options = {
            ...options,
            method: "POST",
            url: `${res.locals.base_url}/api/saes`,
        }
    }
    
    try {
        const result = await axios(options);
        ressource = result.data
    } catch (e) {
        listErrors = e.response.data.errors
        ressource = e.response.data.ressource || {}
    } finally {
        if (listErrors.length || isEdit) {
            res.render("pages/back-end/saes/add-edit.twig", {
                sae: ressource,
                list_errors: listErrors,
                is_edit: isEdit,
            });
        } else {
            res.redirect(`${res.locals.admin_url}/saes`);
        }
    }
});

// router.post(
//     `/${base}/:id`, upload.single("image"),
//     async (req, res) => {
//         let sae = null;
//         let listErrors = [];
//         let targetPath = undefined;
//         let imagePayload = {}

//         const uploadedImage = req.file;

//         if (uploadedImage) {
//             const error = imageValidator(uploadedImage);
//             if(error !== null) {
//                 listErrors.push(error)
//             } else {
//                 imagePayload = { image: req.file?.filename }
//                 targetPath = `${res.locals.upload_dir}${uploadedImage.filename}`;
//                 const tempPath = uploadedImage.path;

//                 fs.copyFile(tempPath, targetPath, err => {
//                     listErrors.push(err)
//                 })
//             }
//         }

//     // We check if there's an id in the url
//     const isEdit = mongoose.Types.ObjectId.isValid(req.params.id)

//     if (isEdit) {
//         sae = await SAE.findByIdAndUpdate(
//             req.params.id,
//             { ...req.body, _id: req.params.id, ...imagePayload },
//             { new: true }
//         )
//             .orFail()
//             .catch((err) => {
//                 if(targetPath) {
//                     fs.unlink(targetPath, (err) => {
//                         listErrors.push(err)
//                     })
//                 }
//                 listErrors = [
//                     ...listErrors,
//                     ...Object.values(err?.errors).map((val) => val.message),
//                 ];
//             });
//     } else {
//         sae = new SAE({ ...req.body, ...imagePayload });

//         await sae
//             .save()
//             .then()
//             .catch((err) => {
//                 if(targetPath) {
//                     fs.unlink(targetPath, (err) => {
//                         listErrors.push(err)
//                     })
//                 }
//                 listErrors = [
//                     ...listErrors,
//                     ...Object.values(err?.errors).map((val) => val.message),
//                 ];
//             });
//     }

//     listErrors = listErrors.filter(Boolean)

//     if (listErrors.length || isEdit) {
//         res.render("pages/back-end/saes/add-edit.twig", {
//             sae: (listErrors.length ? req.body : sae),
//             list_errors: listErrors,
//             is_edit: objectIDRegex.test(req.params.id),
//         });
//     } else {
//         res.redirect(`${res.locals.admin_url}/saes`);
//     }
// });

export default router;
