'use strict';

const gulp = require('gulp');
const build = require('@microsoft/sp-build-web');

// Configure the package-solution task to skip linting
const packageSolutionTask = build.subTask('package-solution', (gulp, buildOptions, done) => {
  buildOptions.args['noLint'] = true;
  build.packages.packageSolution(gulp, buildOptions, done);
});

// Replace the original package-solution task with the modified one
build.rig.addBuildTasks(packageSolutionTask);

// Default build tasks
build.initialize(gulp);
