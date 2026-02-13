import { mapEnumToOptions } from '@abp/ng.core';

export enum ResponseType {
  None = 0,
  Opened = 1,
  Clicked = 2,
  Donated = 3,
  Unsubscribed = 4,
  Bounced = 5,
}

export const responseTypeOptions = mapEnumToOptions(ResponseType);
