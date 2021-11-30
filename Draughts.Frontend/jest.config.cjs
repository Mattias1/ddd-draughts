/** @type {import('ts-jest/dist/types').InitialOptionsTsJest} */
// Apparently this file needs to be run as a CommonJS file, so the extension is .cjs
module.exports = {
  preset: 'ts-jest',
  testEnvironment: 'node',
};
