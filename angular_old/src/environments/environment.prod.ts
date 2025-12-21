import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44318/',
  redirectUri: baseUrl,
  clientId: 'DonaRogApp_App',
  responseType: 'code',
  scope: 'offline_access DonaRogApp',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'DonaRogApp',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44318',
      rootNamespace: 'DonaRogApp',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
