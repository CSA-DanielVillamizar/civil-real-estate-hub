// Debe coincidir con el <title> de index.html — un valor fijo, no
// "document.title" leído en tiempo de import: eso dependía del orden en que
// vitest cargara los módulos de cada archivo de test entre sí, y se rompía
// cuando corrían todos juntos (aunque cada archivo pasara solo).
export const SITE_TITLE = 'Plataforma Civil e Inmobiliaria | Lotes, consultoría e interventoría en Antioquia';
