WitModule = function(_elementId, _witClientKey){
    this.elementId = _elementId
    this.witClientKey = _witClientKey
    this.microphone = new Wit.Microphone(document.getElementById(_elementId))
    
    this.microphone.onconnecting = function() {console.log("Microphone is connecting")}
    this.microphone.ondisconnected = function() {console.log("Microphone is not connected")}
    this.microphone.onerror = function(err) {console.log("Error: " + err)}
    this.microphone.onresult = function(_intent,_entities){
        this.intent = _intent
        this.entities = entities
    }
}

WitModule.prototype.connect = function(){
    this.microphone.connect(this.witClientKey) //identifier of wit.ai, add later  "WCKAJJVRAJPSUGGURT7NFZQDJMLKR4FX"   
}

WitModule.prototype.startRecording = function(){
    this.microphone.start()
}

WitModule.prototype.stopRecording = function(){
    this.microphone.stop()
}

WitModule.prototype.getIntent = function(){
    return this.intent
}

WitModule.prototype.getEntities = function(){
    return this.entities
}