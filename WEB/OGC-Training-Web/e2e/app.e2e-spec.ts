import { OMBOGCWebPage } from './app.po';

describe('ombogc-web App', function() {
  let page: OMBOGCWebPage;

  beforeEach(() => {
    page = new OMBOGCWebPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
