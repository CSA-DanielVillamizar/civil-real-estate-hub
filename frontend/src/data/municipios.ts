// Contenido informativo general sobre normativa de construcción por
// municipio del Oriente Antioqueño — el corredor donde ya se concentra el
// catálogo (fincas de recreo, eco-hoteles, ver docs/02-business-case.md
// §3.1). Cada dato trae su fuente porque esto es información legal-adyacente:
// el POT/las curadurías cambian con el tiempo y publicar una cifra
// desactualizada como si fuera vigente es peor que no publicarla. Por eso
// este archivo NO incluye cifras específicas de retiros/índices de
// construcción por municipio (eso solo lo determina el instrumento de
// ordenamiento vigente para el predio puntual) — solo el marco: quién
// regula, qué instrumento aplica hoy, y quién expide licencias.
//
// Investigado el 2026-09-02. Antes de dar por buena esta información,
// verificar que las fuentes oficiales no hayan cambiado.

export interface Municipio {
  slug: string;
  nombre: string;
  resumen: string;
  pot: {
    instrumento: string;
    estado: string;
    fuenteUrl: string;
    fuenteLabel: string;
  };
  licencias: {
    entidad: string;
    fuenteUrl: string;
    fuenteLabel: string;
  };
  notas?: string;
}

// Las 26 corporaciones autónomas regionales de Colombia dividen el
// territorio por cuencas, no por departamento — los 5 municipios de esta
// lista caen bajo CORNARE (Corporación Autónoma Regional de las Cuencas de
// los Ríos Negro y Nare), la autoridad ambiental de todo el Oriente
// Antioqueño.
// Fuente: https://www.cornare.gov.co/localizacion-regional/
export const AUTORIDAD_AMBIENTAL = {
  nombre: 'CORNARE',
  nombreCompleto: 'Corporación Autónoma Regional de las Cuencas de los Ríos Negro y Nare',
  url: 'https://www.cornare.gov.co/',
};

export const MUNICIPIOS: Municipio[] = [
  {
    slug: 'rionegro',
    nombre: 'Rionegro',
    resumen: 'El municipio más urbanizado del Oriente Antioqueño — sede del aeropuerto José María Córdova y con dos curadurías urbanas propias.',
    pot: {
      instrumento: 'PBOT adoptado por el Acuerdo 056 de 2011, compilado junto con sus modificaciones (Acuerdos 023 de 2012, 028 de 2016 y 002 de 2018) mediante el Decreto 124 de 2018',
      estado: 'Los contenidos de largo plazo vencieron el 31 de diciembre de 2023; el municipio adelanta la Revisión General del PBOT (Decreto Nacional 1232 de 2020).',
      fuenteUrl: 'https://rionegro.gov.co/publicaciones/480/plan-de-ordenamiento-territorial/',
      fuenteLabel: 'Alcaldía de Rionegro — Plan de Ordenamiento Territorial',
    },
    licencias: {
      entidad: 'Curaduría Urbana Primera y Curaduría Urbana Segunda de Rionegro (entidades privadas con función pública)',
      fuenteUrl: 'https://curaduria1rionegro.com/',
      fuenteLabel: 'Curaduría Urbana Primera de Rionegro',
    },
    notas: 'Al tener PBOT con contenidos de largo plazo vencidos y en revisión, la norma urbanística puede tener ajustes en curso — conviene una consulta de uso del suelo reciente antes de comprar.',
  },
  {
    slug: 'la-ceja',
    nombre: 'La Ceja',
    resumen: 'Conocida como "La Ceja del Tambo" — uno de los municipios de mayor producción de flores y viveros del Oriente Antioqueño.',
    pot: {
      instrumento: 'Plan Básico de Ordenamiento Territorial (PBOT) vigente desde 2018',
      estado: 'Vigente.',
      fuenteUrl: 'https://laceja-antioquia.gov.co/Ciudadanos/Paginas/Tramites-y-Servicios-Secretaria-de-Planeacion.aspx',
      fuenteLabel: 'Alcaldía de La Ceja — Trámites y Servicios de Planeación',
    },
    licencias: {
      entidad: 'Secretaría de Planeación Municipal (La Ceja no tiene curaduría urbana propia)',
      fuenteUrl: 'https://laceja-antioquia.gov.co/Ciudadanos/Paginas/Tramites-y-Servicios-Secretaria-de-Planeacion.aspx',
      fuenteLabel: 'Alcaldía de La Ceja — Secretaría de Planeación',
    },
  },
  {
    slug: 'el-retiro',
    nombre: 'El Retiro',
    resumen: 'Uno de los destinos más buscados para fincas de recreo y parcelaciones campestres cerca de Medellín.',
    pot: {
      instrumento: 'Plan Básico de Ordenamiento Territorial, revisado y ajustado tras el Acuerdo original 20 de 1999',
      estado: 'Vigente (con ajustes posteriores al acuerdo original).',
      fuenteUrl: 'https://www.suenosytierras.com/biblioteca/El-Retiro-P.B.O.T-Acuerdo-No.014-2013.pdf',
      fuenteLabel: 'PBOT El Retiro — Acuerdo 014 de 2013 (texto del acuerdo)',
    },
    licencias: {
      entidad: 'Secretaría de Planeación Municipal (El Retiro no tiene curaduría urbana propia)',
      fuenteUrl: 'https://www.elretiro-antioquia.gov.co/',
      fuenteLabel: 'Alcaldía de El Retiro',
    },
    notas: 'Por su alta demanda de parcelaciones campestres, El Retiro tiene restricciones estrictas sobre densidad y área mínima de lote en suelo rural — verificar la categoría de uso del suelo del predio específico antes de comprar.',
  },
  {
    slug: 'guarne',
    nombre: 'Guarne',
    resumen: 'Puerta de entrada al Oriente Antioqueño desde Medellín, con una norma urbanística más restrictiva que la de municipios vecinos.',
    pot: {
      instrumento: 'Plan Básico de Ordenamiento Territorial adoptado por el Acuerdo 003 de 2015 (revisión del Acuerdo 061 de 2000)',
      estado: 'Vigente.',
      fuenteUrl: 'https://www.guarne-antioquia.gov.co/documentos/422/linea-planeacion-y-ordenamiento-territorial/',
      fuenteLabel: 'Alcaldía de Guarne — Planeación y Ordenamiento Territorial',
    },
    licencias: {
      entidad: 'Secretaría de Planeación Municipal (Guarne no tiene curaduría urbana propia)',
      fuenteUrl: 'https://www.guarne-antioquia.gov.co/',
      fuenteLabel: 'Alcaldía de Guarne',
    },
    notas: 'El Acuerdo 003 de 2015 fija un área mínima de 2.000 m² para proyectos de desarrollo urbanístico — una de las normas más restrictivas del Oriente Antioqueño en este punto.',
  },
  {
    slug: 'marinilla',
    nombre: 'Marinilla',
    resumen: 'Centro histórico y comercial del Altiplano del Oriente Antioqueño, con un PBOT actualizado recientemente.',
    pot: {
      instrumento: 'Plan Básico de Ordenamiento Territorial 2022–2035, adoptado por el Acuerdo 07 de 2022 (reemplaza el Acuerdo 98 de 2007)',
      estado: 'Vigente — es el instrumento más reciente de los 5 municipios de esta lista.',
      fuenteUrl: 'https://www.marinilla-antioquia.gov.co/planes/plan-basico-de-ordenamiento-territorial-marinilla-antioquia',
      fuenteLabel: 'Alcaldía de Marinilla — PBOT 2022-2035',
    },
    licencias: {
      entidad: 'Secretaría de Planeación Municipal (Marinilla no tiene curaduría urbana propia)',
      fuenteUrl: 'https://www.marinilla-antioquia.gov.co/',
      fuenteLabel: 'Alcaldía de Marinilla',
    },
  },
];

export function getMunicipioBySlug(slug: string): Municipio | undefined {
  return MUNICIPIOS.find((m) => m.slug === slug);
}
