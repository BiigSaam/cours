import mongoose, { Schema } from "mongoose";

import { isEmptyValidator } from '../validator.js'

const articleSchema = new Schema({
  title: String,
  chapo: String,
  content: String,
  image: String,
  yt_link: String,
}, { timestamps: true });

articleSchema.path("title").validate(isEmptyValidator, "Veuillez mettre un titre, le champ ne peut pas être nul")

articleSchema.pre('findOneAndUpdate', function(next) {
    this.options.runValidators = true;
    next();
});


export default mongoose.model("Article", articleSchema);
