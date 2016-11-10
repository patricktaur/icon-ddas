/**
 * This file is generated by the Angular 2 template compiler.
 * Do not edit.
 */
 /* tslint:disable */

import * as import0 from '@angular/core/src/linker/ng_module_factory';
import * as import1 from '../../app/app.module';
import * as import2 from '@angular/common/src/common_module';
import * as import3 from '@angular/core/src/application_module';
import * as import4 from '@angular/platform-browser/src/browser';
import * as import5 from '@angular/forms/src/directives';
import * as import6 from '@angular/forms/src/form_providers';
import * as import7 from '@angular/http/src/http_module';
import * as import8 from '@angular/router/src/router_module';
import * as import9 from '../../app/shared/utils/ng2-uploader1/ng2-uploader';
import * as import10 from '../../app/search/search.module';
import * as import11 from '@angular/common/src/localization';
import * as import12 from '@angular/core/src/application_init';
import * as import13 from '@angular/core/src/testability/testability';
import * as import14 from '@angular/core/src/application_ref';
import * as import15 from '@angular/core/src/linker/compiler';
import * as import16 from '@angular/platform-browser/src/dom/events/hammer_gestures';
import * as import17 from '@angular/platform-browser/src/dom/events/event_manager';
import * as import18 from '@angular/platform-browser/src/dom/shared_styles_host';
import * as import19 from '@angular/platform-browser/src/dom/dom_renderer';
import * as import20 from '@angular/platform-browser/src/security/dom_sanitization_service';
import * as import21 from '@angular/core/src/linker/view_utils';
import * as import22 from '@angular/platform-browser/src/browser/title';
import * as import23 from '@angular/forms/src/directives/radio_control_value_accessor';
import * as import24 from '@angular/http/src/backends/browser_xhr';
import * as import25 from '@angular/http/src/base_response_options';
import * as import26 from '@angular/http/src/backends/xhr_backend';
import * as import27 from '@angular/http/src/base_request_options';
import * as import28 from '@angular/common/src/location/location';
import * as import29 from '@angular/router/src/url_tree';
import * as import30 from '@angular/router/src/router_outlet_map';
import * as import31 from '@angular/core/src/linker/system_js_ng_module_factory_loader';
import * as import32 from '@angular/router/src/router_preloader';
import * as import33 from '../../app/shared/utils/config.service';
import * as import34 from '../../app/auth/auth.service';
import * as import35 from '../../app/auth/auth-guard.service';
import * as import36 from '../../app/shared/services/can-deactivate-guard.service';
import * as import37 from '../../app/shared/utils/dialog.service';
import * as import38 from '../../app/search/search-service';
import * as import39 from '@angular/core/src/di/injector';
import * as import40 from './search/search.component.ngfactory';
import * as import41 from './search/search-input.component.ngfactory';
import * as import42 from './search/search-detail.component.ngfactory';
import * as import43 from './search/search-result-summary.component.ngfactory';
import * as import44 from './report-temp.component.ngfactory';
import * as import45 from './auth/login.component.ngfactory';
import * as import46 from './app.component.ngfactory';
import * as import47 from '@angular/core/src/application_tokens';
import * as import48 from '@angular/platform-browser/src/dom/events/dom_events';
import * as import49 from '@angular/platform-browser/src/dom/events/key_events';
import * as import50 from '@angular/core/src/zone/ng_zone';
import * as import51 from '@angular/platform-browser/src/dom/debug/ng_probe';
import * as import52 from '../../app/search/search.component';
import * as import53 from '../../app/search/search-input.component';
import * as import54 from '../../app/search/search-detail.component';
import * as import55 from '../../app/search/search-result-summary.component';
import * as import56 from '../../app/report-temp.component';
import * as import57 from '../../app/auth/login.component';
import * as import58 from '@angular/common/src/location/platform_location';
import * as import59 from '@angular/common/src/location/location_strategy';
import * as import60 from '@angular/router/src/router';
import * as import61 from '@angular/core/src/console';
import * as import62 from '@angular/core/src/i18n/tokens';
import * as import63 from '@angular/core/src/error_handler';
import * as import64 from '@angular/platform-browser/src/dom/dom_tokens';
import * as import65 from '@angular/platform-browser/src/dom/animation_driver';
import * as import66 from '@angular/core/src/render/api';
import * as import67 from '@angular/core/src/security';
import * as import68 from '@angular/core/src/change_detection/differs/iterable_differs';
import * as import69 from '@angular/core/src/change_detection/differs/keyvalue_differs';
import * as import70 from '@angular/http/src/interfaces';
import * as import71 from '@angular/http/src/http';
import * as import72 from '@angular/router/src/router_config_loader';
import * as import73 from '@angular/core/src/linker/ng_module_factory_loader';
import * as import74 from '@angular/router/src/router_state';
class AppModuleInjector extends import0.NgModuleInjector<import1.AppModule> {
  _CommonModule_0:import2.CommonModule;
  _ApplicationModule_1:import3.ApplicationModule;
  _BrowserModule_2:import4.BrowserModule;
  _InternalFormsSharedModule_3:import5.InternalFormsSharedModule;
  _FormsModule_4:import6.FormsModule;
  _HttpModule_5:import7.HttpModule;
  _ROUTER_FORROOT_GUARD_6:any;
  _RouterModule_7:import8.RouterModule;
  _Ng2Uploader_8:import9.Ng2Uploader;
  _SearchModule_9:import10.SearchModule;
  _AppModule_10:import1.AppModule;
  __LOCALE_ID_11:any;
  __NgLocalization_12:import11.NgLocaleLocalization;
  _ErrorHandler_13:any;
  _ApplicationInitStatus_14:import12.ApplicationInitStatus;
  _Testability_15:import13.Testability;
  _ApplicationRef__16:import14.ApplicationRef_;
  __ApplicationRef_17:any;
  __Compiler_18:import15.Compiler;
  __APP_ID_19:any;
  __DOCUMENT_20:any;
  __HAMMER_GESTURE_CONFIG_21:import16.HammerGestureConfig;
  __EVENT_MANAGER_PLUGINS_22:any[];
  __EventManager_23:import17.EventManager;
  __DomSharedStylesHost_24:import18.DomSharedStylesHost;
  __AnimationDriver_25:any;
  __DomRootRenderer_26:import19.DomRootRenderer_;
  __RootRenderer_27:any;
  __DomSanitizer_28:import20.DomSanitizerImpl;
  __Sanitizer_29:any;
  __ViewUtils_30:import21.ViewUtils;
  __IterableDiffers_31:any;
  __KeyValueDiffers_32:any;
  __SharedStylesHost_33:any;
  __Title_34:import22.Title;
  __RadioControlRegistry_35:import23.RadioControlRegistry;
  __BrowserXhr_36:import24.BrowserXhr;
  __ResponseOptions_37:import25.BaseResponseOptions;
  __XSRFStrategy_38:any;
  __XHRBackend_39:import26.XHRBackend;
  __RequestOptions_40:import27.BaseRequestOptions;
  __Http_41:any;
  __ROUTES_42:any[];
  __ROUTER_CONFIGURATION_43:any;
  __LocationStrategy_44:any;
  __Location_45:import28.Location;
  __UrlSerializer_46:import29.DefaultUrlSerializer;
  __RouterOutletMap_47:import30.RouterOutletMap;
  __NgModuleFactoryLoader_48:import31.SystemJsNgModuleLoader;
  __Router_49:any;
  __ActivatedRoute_50:any;
  _NoPreloading_51:import32.NoPreloading;
  _PreloadingStrategy_52:any;
  _RouterPreloader_53:import32.RouterPreloader;
  __PreloadAllModules_54:import32.PreloadAllModules;
  __APP_BOOTSTRAP_LISTENER_55:any[];
  __ConfigService_56:import33.ConfigService;
  __AuthService_57:import34.AuthService;
  __AuthGuard_58:import35.AuthGuard;
  __CanDeactivateGuard_59:import36.CanDeactivateGuard;
  __DialogService_60:import37.DialogService;
  __SearchService_61:import38.SearchService;
  constructor(parent:import39.Injector) {
    super(parent,[
      import40.SearchComponentNgFactory,
      import41.SearchInputComponentNgFactory,
      import42.SearchDetailComponentNgFactory,
      import43.SearchResultSummaryComponentNgFactory,
      import44.ReportTempComponentNgFactory,
      import45.LoginComponentNgFactory,
      import46.AppComponentNgFactory
    ]
    ,[import46.AppComponentNgFactory]);
  }
  get _LOCALE_ID_11():any {
    if ((this.__LOCALE_ID_11 == (null as any))) { (this.__LOCALE_ID_11 = 'en-US'); }
    return this.__LOCALE_ID_11;
  }
  get _NgLocalization_12():import11.NgLocaleLocalization {
    if ((this.__NgLocalization_12 == (null as any))) { (this.__NgLocalization_12 = new import11.NgLocaleLocalization(this._LOCALE_ID_11)); }
    return this.__NgLocalization_12;
  }
  get _ApplicationRef_17():any {
    if ((this.__ApplicationRef_17 == (null as any))) { (this.__ApplicationRef_17 = this._ApplicationRef__16); }
    return this.__ApplicationRef_17;
  }
  get _Compiler_18():import15.Compiler {
    if ((this.__Compiler_18 == (null as any))) { (this.__Compiler_18 = new import15.Compiler()); }
    return this.__Compiler_18;
  }
  get _APP_ID_19():any {
    if ((this.__APP_ID_19 == (null as any))) { (this.__APP_ID_19 = import47._appIdRandomProviderFactory()); }
    return this.__APP_ID_19;
  }
  get _DOCUMENT_20():any {
    if ((this.__DOCUMENT_20 == (null as any))) { (this.__DOCUMENT_20 = import4._document()); }
    return this.__DOCUMENT_20;
  }
  get _HAMMER_GESTURE_CONFIG_21():import16.HammerGestureConfig {
    if ((this.__HAMMER_GESTURE_CONFIG_21 == (null as any))) { (this.__HAMMER_GESTURE_CONFIG_21 = new import16.HammerGestureConfig()); }
    return this.__HAMMER_GESTURE_CONFIG_21;
  }
  get _EVENT_MANAGER_PLUGINS_22():any[] {
    if ((this.__EVENT_MANAGER_PLUGINS_22 == (null as any))) { (this.__EVENT_MANAGER_PLUGINS_22 = [
      new import48.DomEventsPlugin(),
      new import49.KeyEventsPlugin(),
      new import16.HammerGesturesPlugin(this._HAMMER_GESTURE_CONFIG_21)
    ]
    ); }
    return this.__EVENT_MANAGER_PLUGINS_22;
  }
  get _EventManager_23():import17.EventManager {
    if ((this.__EventManager_23 == (null as any))) { (this.__EventManager_23 = new import17.EventManager(this._EVENT_MANAGER_PLUGINS_22,this.parent.get(import50.NgZone))); }
    return this.__EventManager_23;
  }
  get _DomSharedStylesHost_24():import18.DomSharedStylesHost {
    if ((this.__DomSharedStylesHost_24 == (null as any))) { (this.__DomSharedStylesHost_24 = new import18.DomSharedStylesHost(this._DOCUMENT_20)); }
    return this.__DomSharedStylesHost_24;
  }
  get _AnimationDriver_25():any {
    if ((this.__AnimationDriver_25 == (null as any))) { (this.__AnimationDriver_25 = import4._resolveDefaultAnimationDriver()); }
    return this.__AnimationDriver_25;
  }
  get _DomRootRenderer_26():import19.DomRootRenderer_ {
    if ((this.__DomRootRenderer_26 == (null as any))) { (this.__DomRootRenderer_26 = new import19.DomRootRenderer_(this._DOCUMENT_20,this._EventManager_23,this._DomSharedStylesHost_24,this._AnimationDriver_25)); }
    return this.__DomRootRenderer_26;
  }
  get _RootRenderer_27():any {
    if ((this.__RootRenderer_27 == (null as any))) { (this.__RootRenderer_27 = import51._createConditionalRootRenderer(this._DomRootRenderer_26,this.parent.get(import51.NgProbeToken,(null as any)))); }
    return this.__RootRenderer_27;
  }
  get _DomSanitizer_28():import20.DomSanitizerImpl {
    if ((this.__DomSanitizer_28 == (null as any))) { (this.__DomSanitizer_28 = new import20.DomSanitizerImpl()); }
    return this.__DomSanitizer_28;
  }
  get _Sanitizer_29():any {
    if ((this.__Sanitizer_29 == (null as any))) { (this.__Sanitizer_29 = this._DomSanitizer_28); }
    return this.__Sanitizer_29;
  }
  get _ViewUtils_30():import21.ViewUtils {
    if ((this.__ViewUtils_30 == (null as any))) { (this.__ViewUtils_30 = new import21.ViewUtils(this._RootRenderer_27,this._APP_ID_19,this._Sanitizer_29)); }
    return this.__ViewUtils_30;
  }
  get _IterableDiffers_31():any {
    if ((this.__IterableDiffers_31 == (null as any))) { (this.__IterableDiffers_31 = import3._iterableDiffersFactory()); }
    return this.__IterableDiffers_31;
  }
  get _KeyValueDiffers_32():any {
    if ((this.__KeyValueDiffers_32 == (null as any))) { (this.__KeyValueDiffers_32 = import3._keyValueDiffersFactory()); }
    return this.__KeyValueDiffers_32;
  }
  get _SharedStylesHost_33():any {
    if ((this.__SharedStylesHost_33 == (null as any))) { (this.__SharedStylesHost_33 = this._DomSharedStylesHost_24); }
    return this.__SharedStylesHost_33;
  }
  get _Title_34():import22.Title {
    if ((this.__Title_34 == (null as any))) { (this.__Title_34 = new import22.Title()); }
    return this.__Title_34;
  }
  get _RadioControlRegistry_35():import23.RadioControlRegistry {
    if ((this.__RadioControlRegistry_35 == (null as any))) { (this.__RadioControlRegistry_35 = new import23.RadioControlRegistry()); }
    return this.__RadioControlRegistry_35;
  }
  get _BrowserXhr_36():import24.BrowserXhr {
    if ((this.__BrowserXhr_36 == (null as any))) { (this.__BrowserXhr_36 = new import24.BrowserXhr()); }
    return this.__BrowserXhr_36;
  }
  get _ResponseOptions_37():import25.BaseResponseOptions {
    if ((this.__ResponseOptions_37 == (null as any))) { (this.__ResponseOptions_37 = new import25.BaseResponseOptions()); }
    return this.__ResponseOptions_37;
  }
  get _XSRFStrategy_38():any {
    if ((this.__XSRFStrategy_38 == (null as any))) { (this.__XSRFStrategy_38 = import7._createDefaultCookieXSRFStrategy()); }
    return this.__XSRFStrategy_38;
  }
  get _XHRBackend_39():import26.XHRBackend {
    if ((this.__XHRBackend_39 == (null as any))) { (this.__XHRBackend_39 = new import26.XHRBackend(this._BrowserXhr_36,this._ResponseOptions_37,this._XSRFStrategy_38)); }
    return this.__XHRBackend_39;
  }
  get _RequestOptions_40():import27.BaseRequestOptions {
    if ((this.__RequestOptions_40 == (null as any))) { (this.__RequestOptions_40 = new import27.BaseRequestOptions()); }
    return this.__RequestOptions_40;
  }
  get _Http_41():any {
    if ((this.__Http_41 == (null as any))) { (this.__Http_41 = import7.httpFactory(this._XHRBackend_39,this._RequestOptions_40)); }
    return this.__Http_41;
  }
  get _ROUTES_42():any[] {
    if ((this.__ROUTES_42 == (null as any))) { (this.__ROUTES_42 = [
      [
        {
          path: '',
          redirectTo: '/search',
          pathMatch: 'full'
        }
        ,
        {
          path: 'search',
          component: import52.SearchComponent,
          canActivate: [import35.AuthGuard],
          children: [
            {
              path: '',
              component: import53.SearchInputComponent
            }
            ,
            {
              path: 'details/:siteEnum/:name/:id',
              component: import54.SearchDetailComponent
            }
            ,
            {
              path: 'summary/:name',
              component: import55.SearchResultSummaryComponent
            }

          ]

        }

      ]
      ,
      [
        {
          path: 'report',
          component: import56.ReportTempComponent,
          canActivate: [import35.AuthGuard]
        }
        ,
        {
          path: 'login',
          component: import57.LoginComponent
        }
        ,
        {
          path: 'logout',
          redirectTo: 'login'
        }

      ]

    ]
    ); }
    return this.__ROUTES_42;
  }
  get _ROUTER_CONFIGURATION_43():any {
    if ((this.__ROUTER_CONFIGURATION_43 == (null as any))) { (this.__ROUTER_CONFIGURATION_43 = {}); }
    return this.__ROUTER_CONFIGURATION_43;
  }
  get _LocationStrategy_44():any {
    if ((this.__LocationStrategy_44 == (null as any))) { (this.__LocationStrategy_44 = import8.provideLocationStrategy(this.parent.get(import58.PlatformLocation),this.parent.get(import59.APP_BASE_HREF,(null as any)),this._ROUTER_CONFIGURATION_43)); }
    return this.__LocationStrategy_44;
  }
  get _Location_45():import28.Location {
    if ((this.__Location_45 == (null as any))) { (this.__Location_45 = new import28.Location(this._LocationStrategy_44)); }
    return this.__Location_45;
  }
  get _UrlSerializer_46():import29.DefaultUrlSerializer {
    if ((this.__UrlSerializer_46 == (null as any))) { (this.__UrlSerializer_46 = new import29.DefaultUrlSerializer()); }
    return this.__UrlSerializer_46;
  }
  get _RouterOutletMap_47():import30.RouterOutletMap {
    if ((this.__RouterOutletMap_47 == (null as any))) { (this.__RouterOutletMap_47 = new import30.RouterOutletMap()); }
    return this.__RouterOutletMap_47;
  }
  get _NgModuleFactoryLoader_48():import31.SystemJsNgModuleLoader {
    if ((this.__NgModuleFactoryLoader_48 == (null as any))) { (this.__NgModuleFactoryLoader_48 = new import31.SystemJsNgModuleLoader(this._Compiler_18,this.parent.get(import31.SystemJsNgModuleLoaderConfig,(null as any)))); }
    return this.__NgModuleFactoryLoader_48;
  }
  get _Router_49():any {
    if ((this.__Router_49 == (null as any))) { (this.__Router_49 = import8.setupRouter(this._ApplicationRef_17,this._UrlSerializer_46,this._RouterOutletMap_47,this._Location_45,this,this._NgModuleFactoryLoader_48,this._Compiler_18,this._ROUTES_42,this._ROUTER_CONFIGURATION_43)); }
    return this.__Router_49;
  }
  get _ActivatedRoute_50():any {
    if ((this.__ActivatedRoute_50 == (null as any))) { (this.__ActivatedRoute_50 = import8.rootRoute(this._Router_49)); }
    return this.__ActivatedRoute_50;
  }
  get _PreloadAllModules_54():import32.PreloadAllModules {
    if ((this.__PreloadAllModules_54 == (null as any))) { (this.__PreloadAllModules_54 = new import32.PreloadAllModules()); }
    return this.__PreloadAllModules_54;
  }
  get _APP_BOOTSTRAP_LISTENER_55():any[] {
    if ((this.__APP_BOOTSTRAP_LISTENER_55 == (null as any))) { (this.__APP_BOOTSTRAP_LISTENER_55 = [import8.initialRouterNavigation(this._Router_49,this._ApplicationRef_17,this._RouterPreloader_53,this._ROUTER_CONFIGURATION_43)]); }
    return this.__APP_BOOTSTRAP_LISTENER_55;
  }
  get _ConfigService_56():import33.ConfigService {
    if ((this.__ConfigService_56 == (null as any))) { (this.__ConfigService_56 = new import33.ConfigService()); }
    return this.__ConfigService_56;
  }
  get _AuthService_57():import34.AuthService {
    if ((this.__AuthService_57 == (null as any))) { (this.__AuthService_57 = new import34.AuthService(this._Http_41,this._ConfigService_56)); }
    return this.__AuthService_57;
  }
  get _AuthGuard_58():import35.AuthGuard {
    if ((this.__AuthGuard_58 == (null as any))) { (this.__AuthGuard_58 = new import35.AuthGuard(this._AuthService_57,this._Router_49)); }
    return this.__AuthGuard_58;
  }
  get _CanDeactivateGuard_59():import36.CanDeactivateGuard {
    if ((this.__CanDeactivateGuard_59 == (null as any))) { (this.__CanDeactivateGuard_59 = new import36.CanDeactivateGuard()); }
    return this.__CanDeactivateGuard_59;
  }
  get _DialogService_60():import37.DialogService {
    if ((this.__DialogService_60 == (null as any))) { (this.__DialogService_60 = new import37.DialogService()); }
    return this.__DialogService_60;
  }
  get _SearchService_61():import38.SearchService {
    if ((this.__SearchService_61 == (null as any))) { (this.__SearchService_61 = new import38.SearchService(this._Http_41,this._ConfigService_56)); }
    return this.__SearchService_61;
  }
  createInternal():import1.AppModule {
    this._CommonModule_0 = new import2.CommonModule();
    this._ApplicationModule_1 = new import3.ApplicationModule();
    this._BrowserModule_2 = new import4.BrowserModule(this.parent.get(import4.BrowserModule,(null as any)));
    this._InternalFormsSharedModule_3 = new import5.InternalFormsSharedModule();
    this._FormsModule_4 = new import6.FormsModule();
    this._HttpModule_5 = new import7.HttpModule();
    this._ROUTER_FORROOT_GUARD_6 = import8.provideForRootGuard(this.parent.get(import60.Router,(null as any)));
    this._RouterModule_7 = new import8.RouterModule(this._ROUTER_FORROOT_GUARD_6);
    this._Ng2Uploader_8 = new import9.Ng2Uploader();
    this._SearchModule_9 = new import10.SearchModule();
    this._AppModule_10 = new import1.AppModule();
    this._ErrorHandler_13 = import4.errorHandler();
    this._ApplicationInitStatus_14 = new import12.ApplicationInitStatus(this.parent.get(import12.APP_INITIALIZER,(null as any)));
    this._Testability_15 = new import13.Testability(this.parent.get(import50.NgZone));
    this._ApplicationRef__16 = new import14.ApplicationRef_(this.parent.get(import50.NgZone),this.parent.get(import61.Console),this,this._ErrorHandler_13,this,this._ApplicationInitStatus_14,this.parent.get(import13.TestabilityRegistry,(null as any)),this._Testability_15);
    this._NoPreloading_51 = new import32.NoPreloading();
    this._PreloadingStrategy_52 = this._NoPreloading_51;
    this._RouterPreloader_53 = new import32.RouterPreloader(this._Router_49,this._NgModuleFactoryLoader_48,this._Compiler_18,this,this._PreloadingStrategy_52);
    return this._AppModule_10;
  }
  getInternal(token:any,notFoundResult:any):any {
    if ((token === import2.CommonModule)) { return this._CommonModule_0; }
    if ((token === import3.ApplicationModule)) { return this._ApplicationModule_1; }
    if ((token === import4.BrowserModule)) { return this._BrowserModule_2; }
    if ((token === import5.InternalFormsSharedModule)) { return this._InternalFormsSharedModule_3; }
    if ((token === import6.FormsModule)) { return this._FormsModule_4; }
    if ((token === import7.HttpModule)) { return this._HttpModule_5; }
    if ((token === import8.ROUTER_FORROOT_GUARD)) { return this._ROUTER_FORROOT_GUARD_6; }
    if ((token === import8.RouterModule)) { return this._RouterModule_7; }
    if ((token === import9.Ng2Uploader)) { return this._Ng2Uploader_8; }
    if ((token === import10.SearchModule)) { return this._SearchModule_9; }
    if ((token === import1.AppModule)) { return this._AppModule_10; }
    if ((token === import62.LOCALE_ID)) { return this._LOCALE_ID_11; }
    if ((token === import11.NgLocalization)) { return this._NgLocalization_12; }
    if ((token === import63.ErrorHandler)) { return this._ErrorHandler_13; }
    if ((token === import12.ApplicationInitStatus)) { return this._ApplicationInitStatus_14; }
    if ((token === import13.Testability)) { return this._Testability_15; }
    if ((token === import14.ApplicationRef_)) { return this._ApplicationRef__16; }
    if ((token === import14.ApplicationRef)) { return this._ApplicationRef_17; }
    if ((token === import15.Compiler)) { return this._Compiler_18; }
    if ((token === import47.APP_ID)) { return this._APP_ID_19; }
    if ((token === import64.DOCUMENT)) { return this._DOCUMENT_20; }
    if ((token === import16.HAMMER_GESTURE_CONFIG)) { return this._HAMMER_GESTURE_CONFIG_21; }
    if ((token === import17.EVENT_MANAGER_PLUGINS)) { return this._EVENT_MANAGER_PLUGINS_22; }
    if ((token === import17.EventManager)) { return this._EventManager_23; }
    if ((token === import18.DomSharedStylesHost)) { return this._DomSharedStylesHost_24; }
    if ((token === import65.AnimationDriver)) { return this._AnimationDriver_25; }
    if ((token === import19.DomRootRenderer)) { return this._DomRootRenderer_26; }
    if ((token === import66.RootRenderer)) { return this._RootRenderer_27; }
    if ((token === import20.DomSanitizer)) { return this._DomSanitizer_28; }
    if ((token === import67.Sanitizer)) { return this._Sanitizer_29; }
    if ((token === import21.ViewUtils)) { return this._ViewUtils_30; }
    if ((token === import68.IterableDiffers)) { return this._IterableDiffers_31; }
    if ((token === import69.KeyValueDiffers)) { return this._KeyValueDiffers_32; }
    if ((token === import18.SharedStylesHost)) { return this._SharedStylesHost_33; }
    if ((token === import22.Title)) { return this._Title_34; }
    if ((token === import23.RadioControlRegistry)) { return this._RadioControlRegistry_35; }
    if ((token === import24.BrowserXhr)) { return this._BrowserXhr_36; }
    if ((token === import25.ResponseOptions)) { return this._ResponseOptions_37; }
    if ((token === import70.XSRFStrategy)) { return this._XSRFStrategy_38; }
    if ((token === import26.XHRBackend)) { return this._XHRBackend_39; }
    if ((token === import27.RequestOptions)) { return this._RequestOptions_40; }
    if ((token === import71.Http)) { return this._Http_41; }
    if ((token === import72.ROUTES)) { return this._ROUTES_42; }
    if ((token === import8.ROUTER_CONFIGURATION)) { return this._ROUTER_CONFIGURATION_43; }
    if ((token === import59.LocationStrategy)) { return this._LocationStrategy_44; }
    if ((token === import28.Location)) { return this._Location_45; }
    if ((token === import29.UrlSerializer)) { return this._UrlSerializer_46; }
    if ((token === import30.RouterOutletMap)) { return this._RouterOutletMap_47; }
    if ((token === import73.NgModuleFactoryLoader)) { return this._NgModuleFactoryLoader_48; }
    if ((token === import60.Router)) { return this._Router_49; }
    if ((token === import74.ActivatedRoute)) { return this._ActivatedRoute_50; }
    if ((token === import32.NoPreloading)) { return this._NoPreloading_51; }
    if ((token === import32.PreloadingStrategy)) { return this._PreloadingStrategy_52; }
    if ((token === import32.RouterPreloader)) { return this._RouterPreloader_53; }
    if ((token === import32.PreloadAllModules)) { return this._PreloadAllModules_54; }
    if ((token === import47.APP_BOOTSTRAP_LISTENER)) { return this._APP_BOOTSTRAP_LISTENER_55; }
    if ((token === import33.ConfigService)) { return this._ConfigService_56; }
    if ((token === import34.AuthService)) { return this._AuthService_57; }
    if ((token === import35.AuthGuard)) { return this._AuthGuard_58; }
    if ((token === import36.CanDeactivateGuard)) { return this._CanDeactivateGuard_59; }
    if ((token === import37.DialogService)) { return this._DialogService_60; }
    if ((token === import38.SearchService)) { return this._SearchService_61; }
    return notFoundResult;
  }
  destroyInternal():void {
    this._ApplicationRef__16.ngOnDestroy();
    this._RouterPreloader_53.ngOnDestroy();
  }
}
export const AppModuleNgFactory:import0.NgModuleFactory<import1.AppModule> = new import0.NgModuleFactory(AppModuleInjector,import1.AppModule);