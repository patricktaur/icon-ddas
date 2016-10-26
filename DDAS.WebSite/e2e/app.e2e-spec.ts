import { DdasPage } from './app.po';

describe('ddas App', function() {
  let page: DdasPage;

  beforeEach(() => {
    page = new DdasPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
