import express from "express";
import mongoose from "mongoose";

import CommentArticle from '#models/comment-article.js'
import Article from '#models/article.js'

const router = express.Router();
const base = "articles";

/**
 * @openapi
 * /articles/{id}/comments:
 *   post:
 *     tags:
 *      - Articles
 *     parameters:
 *      - name: id
 *        in: path
 *        description: Article's _id
 *        required: true
 *        schema:
 *          type: string
 *          pattern: '([0-9a-f]{24})'
 *     requestBody:
 *        content:
 *          application/json:
 *              schema:
 *                  type: object
 *                  required: ["content"]
 *                  properties:
 *                    content:
 *                      type: string
 *     responses:
 *       201:
 *         description: Creates a comment for an article
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/CommentArticle'
 *       400:
 *         description: Something went wrong
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Error'
 *       404:
 *         description: Ressource not found
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Error'
 */
router.post(`/${base}/:id/comments`, async (req, res) => {
    try {
        const article = await Article.findById(req.params.id)
        const ressource = new CommentArticle({ ...req.body, article });
        await ressource.save()
        
        article.list_comments.push(ressource);
        await article.save();

        res.status(201).json(ressource.getClean())
    } catch (err) {
        if (err instanceof mongoose.Error.DocumentNotFoundError) {
            res.status(404).json({
                errors: [`L'article "${req.params.id}" n'existe pas`],
            });
        } else if(err instanceof mongoose.Error.CastError) {
            res.status(400).json({
                errors: [`"${req.params.id}" n'est pas un _id valide`],
            });
        } else {
            res.status(400).json({
                errors: [
                    ...Object.values(err?.errors || [{'message': "Il y a eu un problème"}]).map((val) => val.message)
                ]
            })
        }
    }
});

/**
 * @openapi
 * /articles/{id}/comments:
 *   get:
 *     tags:
 *      - Articles
 *     summary: Get comments for one article
 *     description: |
 *        Returns 10 by 10 comments related to one article
 *     parameters:
 *      - name: id
 *        in: path
 *        description: article's _id
 *        required: true
 *        schema:
 *          type: string
 *          pattern: '([0-9a-f]{24})'
 *      - in: query
 *        name: page
 *        schema:
 *          type: integer
 *          example: 1
 *        description: Page's number
 *     responses:
 *      200:
 *         description: Get all comments for an article
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/ListCommentsArticle'
 *      400:
 *         description: Something went wrong
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Error'
 *      404:
 *        description: Ressource not found
 *        content:
 *          application/json:
 *            schema:
 *              $ref: '#/components/schemas/Error'
 */
router.get(`/${base}/:id/comments`, async (req, res) => {
    try {
        const page = Math.max(1, req.query.page || 1);
        const perPage = 10;

        const ressource = await Article.aggregate([
            { $match: { _id: new mongoose.Types.ObjectId(req.params.id) } },
            {
                $project: {
                    list_comments: 1,
                },
            },
            {
                $lookup: {
                    from: "commentarticles",
                    localField: "list_comments",
                    foreignField: "_id",
                    as: "list_comments",
                },
            },
            {
                $addFields: {
                    page: page,
                    count: { $size: "$list_comments" },
                    total_pages: {
                        $ceil: {
                            $divide: [{ $size: "$list_comments" }, perPage],
                        },
                    },
                    list_comments: {
                        $slice: [
                            {
                                $sortArray: {
                                    input: "$list_comments",
                                    sortBy: {
                                        created_at: -1,
                                    },
                                },
                            },
                            Math.max(page - 1, 0) * perPage,
                            perPage
                        ],
                    },
                },
            },
        ]);

        if (!ressource.length) {
            return res.status(404).json({
                errors: [`L'article "${req.params.id}" n'existe pas`],
            });
        }

        res.status(200).json(ressource[0]);
    } catch (err) {
        res.status(400).json({
            errors: [
                ...Object.values(
                    err?.errors || [{ message: "Il y a eu un problème" }]
                ).map((val) => val.message),
            ],
        });
    }
});

/**
 * @openapi
 * /articles/{id}/comments:
 *   delete:
 *     tags:
 *      - Articles
 *     summary: Delete a comment
 *     parameters:
 *      - name: id
 *        in: path
 *        description: article's comment's _id
 *        required: true
 *        schema:
 *          type: string
 *          pattern: '([0-9a-f]{24})'
 *     responses:
 *       200:
 *         description: Deletes an article's comment's
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/CommentArticle'
 *       400:
 *         description: Something went wrong
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Error'
 *       404:
 *         description: Ressource not found
 *         content:
 *           application/json:
 *             schema:
 *               $ref: '#/components/schemas/Error'
 */
router.delete(`/${base}/:id/comments`, async (req, res) => {
    try {
        const ressource = await CommentArticle.findByIdAndDelete(req.params.id)

        if (ressource?.image) {
            const targetPath = `${res.locals.upload_dir}${ressource.image}`;
            fs.unlink(targetPath, (err) => {});
        }

        if(ressource) {
            return res.status(200).json(ressource);
        }
        return res.status(404).json({
            errors: [`Le commentaire "${req.params.id}" n'existe pas`],
        });
    } catch (error) {
        return res
            .status(400)
            .json({
                error: "Quelque chose s'est mal passé, veuillez recommencer",
            });
    }
});

export default router;
