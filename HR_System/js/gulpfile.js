﻿var gulp = require('gulp'),
    sass = require('gulp-sass');

gulp.task('build-css', function () {
    return gulp
    .src('./scss/**/*.scss')
    .pipe(sass())
    .pipe(gulp.dest('./css'));
});

gulp.task('default', ['build-css']);