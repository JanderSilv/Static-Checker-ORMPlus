const fs = require('fs');

const readWirthFile = () => {
  const path = './gramatica/wirth_reduzido.txt';
  try {
    const file = fs.readFileSync(path, 'utf-8');
    // console.log('file: ', file);
    return file;
  } catch (error) {
    console.log('erro: ', error);
  }
};

const setDots = () => {
  const wirth = readWirthFile();
  // const section = wirth.split('.')[3];
  const dotReplace = '$1 . ';
  const wirthWithDots = wirth
    .replace(/((\w|-)+\s=)/g, '$1 . ') // Adiciona o ponto inicial
    .replace(/((\w|"[,;><#!*+&=/\-\[\]\(\)\{\}]|(([=!<>])=))")/g, dotReplace) // Não terminais
    .replace(/((\w|-)+)([\)\]\}\s])/g, '$1 . $3') // Terminais
    .replace(/([\(\)\[\]\{\}])/g, dotReplace) // Agrupamentos
    .replace(/(\|)/g, dotReplace) // ORs
    .replace(/"([\(\)\[\]\{\}]) . "/g, '"$1"') // Corrige não terminais com simbolo de agrupamento
    .replace(/\s+/g, ' ')
    .trim() // Remove espaços maiores que 1
    .replace(/\s\.\s\./g, '.') // Remove a sentença ' . . ' do final
    .replace(/((\w|-)+)\s\.(\s=)/g, '$1$3') // Corrige declaração do terminal
    .replace(/((\w|-)+\s=\s\.)/g, '\n\n$1'); // Quebra linha nos terminais
  // console.log('wirthWithDots: ', wirthWithDots);
  return wirthWithDots;
};

const getAutomatonStates = () => {
  const wirthWithDots = setDots();
};

setDots();
