import { OmbogcWebPage } from './app.po';

describe('ombogc-web App', function() {
  let page: OmbogcWebPage;

  beforeEach(() => {
    page = new OmbogcWebPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
