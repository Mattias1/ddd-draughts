import { fileURLToPath as _fileURLToPath } from 'url';
import { dirname as _dirname, resolve as _resolve } from 'path';

const __filename = _fileURLToPath(import.meta.url);
const __dirname = _dirname(__filename);

export default (env) => {
  const isProd = env.production;

  return {
    entry: './ts/index.ts',
    mode: isProd ? 'production' : 'development',
    devtool: isProd ? false : 'inline-source-map',
    module: {
      rules: [{
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/
      }]
    },
    resolve: {
      extensions: ['.ts', '.js', '.tsx']
    },
    output: {
      filename: 'bundle.js',
      path: _resolve(__dirname, '../Draughts/wwwroot/js'),
      library: 'DraughtsApp',
      libraryExport: 'default',
      libraryTarget: 'var'
    },
    watchOptions: {
      poll: true,
      ignored: '/node_modules/',
    }
  };
};
