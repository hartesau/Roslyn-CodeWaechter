Roslyn Code-Wächter v41 - Dependency Scanner Edition
https://www.youtube.com/watch?v=6_auB5QlPl4&t=74s
Der Roslyn CodeWächter ist mehr als nur ein Tool – er ist dein **digitaler Zwilling** deines Projekts. Je länger du ihn nutzt, desto wertvoller wird die Seele-Datenbank und desto sicherer wird deine gesamte Code-Basis.

**Wichtigster Workflow:**

1. Alten Code links laden
2. Neuen Code rechts einfügen
3. **Analyse** starten → prüfen
4. Bei Bedarf **Seele einhauchen** → KI erklären lassen
5. **Compile** → testen
6. **Snapshot** → sichern

# Willkommen im Kommandozentrum des Roslyn Code-Wächters (v46)

## 1. Einführung: Ihr persönlicher KI-Entwicklungs-Bodyguard

Herzlich willkommen zu "Rosie", Ihrem fortschrittlichsten digitalen Assistenten für die Softwareentwicklung. Rosie, der *Roslyn Code-Wächter*, ist weit mehr als nur ein einfaches Programm – sie ist Ihr autonomes Entwicklungsstudio, Ihr unbestechlicher Qualitätskontrolleur und Ihr persönlicher KI-Programmierpartner, alles in einem kompakten, selbstheilenden Paket. Rosie wurde geschaffen, um Ihnen die Kontrolle über Ihren Code zurückzugeben und die Zusammenarbeit mit Künstlicher Intelligenz so sicher und effizient wie nie zuvor zu gestalten.

### 1.1. Was ist der Roslyn Code-Wächter (Rosie)?

Stellen Sie sich Rosie als Ihre persönliche KI-Programmierfabrik vor, die Sie einfach auf einen USB-Stick ziehen und überallhin mitnehmen können. Sie ist ein hochspezialisiertes Werkzeug, das Ihnen dabei hilft:

* **Code zu überprüfen:** Rosie scannt Ihren C#-Code bis ins kleinste Detail, findet versteckte Änderungen und schützt Sie vor Fehlern, die von KIs oder menschlichen Entwicklern eingeschleust werden könnten.
* **Code zu generieren:** Sie können Rosie Anweisungen geben, und sie wird Ihnen fertigen C#-Code liefern, der dann sofort kompilierbar ist.
* **Projekte zu verwalten:** Rosie kann komplette C#-Projekte von Grund auf neu erstellen, alle benötigten Bibliotheken (DLLs) automatisch installieren und sich sogar selbst in das neue Projekt kopieren, um es dort weiter zu betreuen.
* **Sich selbst zu dokumentieren:** Rosie kann sich selbst oder andere Softwareprojekte detailliert und laienverständlich dokumentieren.
* **Ihre Arbeit zu sichern:** Sie speichert jede Aktion, jeden Vergleich und jeden Code-Stand, damit Sie niemals den Überblick verlieren oder Daten unwiederbringlich verloren gehen.

### 1.2. Die Philosophie hinter Rosie: Ihr unbestechlicher Bodyguard

Die Entwicklung von Rosie basiert auf einer Kernphilosophie: **Absolute Kontrolle und Sicherheit**. In der rasanten Welt der KI-generierten Software birgt die Zusammenarbeit mit künstlichen Intelligenzen (KIs) ein Risiko: die sogenannte "KI-Halluzination". KIs können unbeabsichtigt funktionierenden Code "zusammenstampfen" (löschen), fehlerhaften Code generieren oder subtile Bugs einschleusen, die schwer zu entdecken sind. Rosie ist Ihr **Bodyguard** gegen diese Risiken.

Sie agiert als **"unbestechlicher Wächter"** und **"TÜV-Prüfer"** zwischen Ihnen und der KI. Bevor irgendein von einer KI generierter Code in Ihr Projekt gelangt, legt er Rosies gnadenlosen **"Röntgenblick"** frei. Rosie meldet Ihnen jede noch so kleine Abweichung, jeden gelöschten Button, jede geänderte Methode – und das, bevor der Code überhaupt kompiliert wird. Dies ist der **"Heilige Gral"** der KI-gestützten Softwareentwicklung: Vertrauen ist gut, maschineller Beweis ist tausendmal besser.

### 1.3. Der digitale Zwilling: Verstehen, was unter der Haube steckt

Rosie versteht Ihren Code nicht nur als Text, sondern als ein **"digitales Lebewesen"** mit einer komplexen Anatomie. Sie erstellt einen **"digitalen Zwilling"** Ihres Programms – eine detaillierte Karte aller Klassen, Bauteile (Felder) und Methoden, inklusive ihrer "Verdrahtung" (welche Methode ruft welche andere auf). Diese Metadaten sind der **"Bauplan"** Ihrer Software und ermöglichen es Rosie, nicht nur zu prüfen, was sich geändert hat, sondern auch, die **"Seele"** Ihres Codes zu verstehen und zu dokumentieren.

## 2. Grundlagen der Benutzeroberfläche: Ihr Kontrollpanel

Rosie wurde mit dem Gedanken an Effizienz und Übersichtlichkeit entwickelt. Die gesamte Anwendung ist in einem dynamischen Layout aufgebaut, das sich an Ihre Fenstergröße anpasst. Hier ist eine detaillierte Führung durch die Benutzeroberfläche:

### 2.1. Fensteraufbau & Navigation

Das Hauptfenster von Rosie ist in zwei große Bereiche unterteilt, die Sie mithilfe von **Splitter-Balken** in ihrer Größe anpassen können:

* **Oberer Bereich (Code-Labor):** Hier sehen Sie zwei große Textfelder für Ihren C#-Code (Status Quo und Neues Update). Dieser Bereich ist Ihre "Werkbank" für Code-Änderungen.
* **Unterer Bereich (Arbeitsbereiche):** Dieser Bereich enthält eine "Tab-Leiste" mit verschiedenen Reitern wie "Diff-Protokoll", "Architektur & Lexikon" oder "KI-Inception". Jeder Tab dient einem spezifischen Zweck in Ihrem Entwicklungsworkflow.

Sie können die Größe dieser Bereiche einfach ändern, indem Sie die horizontalen oder vertikalen Trennbalken mit der Maus ziehen. Rosie merkt sich Ihre bevorzugte Anordnung und stellt sie beim nächsten Start wieder her.

### 2.2. Die obere Werkzeugleiste: Ihre Schnellzugriffszentrale

Über den Code-Fenstern befindet sich die obere Werkzeugleiste, die Ihnen schnellen Zugriff auf Rosies Kernfunktionen bietet. Jeder Button ist ein wichtiges Werkzeug in Ihrem digitalen "Werkzeugkasten":

* **⚡ Analyse (Blau):**
    * **Was es tut:** Dieser Button startet den Kernprozess des Roslyn Code-Wächters. Rosie vergleicht den Code im linken Fenster ("Status Quo") mit dem Code im rechten Fenster ("Neues Update") auf strukturelle Änderungen. Sie nutzt die **Roslyn API** (Microsofts offiziellen C#-Compiler als Bibliothek), um einen detaillierten **Abstrakten Syntaxbaum (AST)** beider Code-Versionen zu erstellen und die Unterschiede zu identifizieren.
    * **Warum es existiert:** Dies ist Ihr **"Röntgenblick"** auf den Code. Er schützt Sie davor, dass KIs oder Sie selbst unbemerkt wichtige Teile des Programms löschen oder verändern. Bevor Sie einen neuen Code kompilieren oder in Ihr Projekt übernehmen, klicken Sie hier, um sicherzustellen, dass keine **"Stampf-Fehler"** passiert sind. Rosie zeigt Ihnen sofort, welche Klassen, Felder oder Methoden hinzugefügt oder entfernt wurden.

* **📜 Audit (Dunkelgrau):**
    * **Was es tut:** Öffnet ein separates Fenster, das eine chronologische Liste aller wichtigen Aktionen anzeigt, die Rosie durchgeführt hat (z.B. wann Code-Backups erstellt wurden, wann der Compiler aufgerufen wurde, etc.).
    * **Warum es existiert:** Dies ist Ihr **"technisches Logbuch"** oder **"Audit-Trail"**. Es dient als lückenlose Dokumentation aller Vorgänge und ist besonders nützlich, um nachzuvollziehen, wann was passiert ist, oder um potenzielle Fehlerquellen zu identifizieren. Es ist der Beweis, dass Rosie Ihre Arbeit aufmerksam verfolgt.

* **📄 Changelog (Mittelgrau):**
    * **Was es tut:** Öffnet ein separates Fenster, das einen menschlich lesbaren Bericht aller strukturellen Code-Änderungen enthält, die durch den "⚡ Analyse"-Button protokolliert wurden (z.B. Hinzufügen eines Buttons, Löschen einer Methode).
    * **Warum es existiert:** Dies ist Ihr **"Entwicklungs-Tagebuch"**. Im Gegensatz zum technischen Audit-Log konzentriert sich das Changelog auf die Evolution Ihres Codes auf einer abstrakteren Ebene. Es ist wie eine kurze Zusammenfassung dessen, was sich an der "Architektur" Ihrer App geändert hat, ideal für die Nachvollziehbarkeit von Features und Bugfixes.

* **ℹ️ Info (Braun/Gelb):**
    * **Was es tut:** Öffnet ein separates Fenster, das dieses Anwenderhandbuch anzeigt. Das Handbuch ist direkt in der App bearbeitbar und kann mit `Strg+S` gespeichert werden.
    * **Warum es existiert:** Dies ist Ihre eingebaute **"Wissensdatenbank"**. Hier finden Sie jederzeit alle Informationen zur Bedienung von Rosie. Die Bearbeitbarkeit ermöglicht es Ihnen, eigene Notizen oder spezifische Projektanweisungen direkt im Handbuch zu hinterlegen.

* **🔨 Build & Neustart (Rot):**
    * **Was es tut:** Dies ist der **"Zündschlüssel"** für Ihr Projekt. Rosie speichert den Code aus dem rechten Fenster ("Neues Update") in die von Ihnen konfigurierte `Program.cs`-Zieldatei, ruft dann Ihr externes Build-Skript (`.bat`) auf, um Ihr Projekt zu kompilieren, und startet danach, falls konfiguriert, die fertige `.exe`-Anwendung neu.
    * **Warum es existiert:** Dieser Button integriert den Kompilierungsprozess nahtlos in Rosies Workflow. Er stellt sicher, dass der von Ihnen oder der KI generierte Code in eine lauffähige Anwendung verwandelt wird. Die "Neustart"-Funktion sorgt dafür, dass Sie die Änderungen Ihres Codes sofort im laufenden Programm sehen können, ohne manuell im Explorer suchen zu müssen.

* **Auto-Exit nach Build (Checkbox):**
    * **Was es tut:** Wenn diese Checkbox aktiviert ist, schließt sich Rosie nach einem erfolgreichen Build-Vorgang automatisch und startet die neue Version Ihrer kompilierten `.exe`.
    * **Warum es existiert:** Dies ist eine **"Workflow-Option"**. Wenn Sie Rosie als reinen Compiler-Trigger nutzen und schnell die neue Version Ihrer App sehen möchten, spart Ihnen das den manuellen Klick. Wenn Sie jedoch im "Marathon-Flow" sind und Rosie für weitere Code-Iterationen offenhalten möchten, deaktivieren Sie diese Option.

* **⚖️ Mitte (Grau):**
    * **Was es tut:** Setzt alle internen Trennbalken in Rosie auf eine gleichmäßige 50%-Position zurück.
    * **Warum es existiert:** Dieser Button ist Ihr **"Reset-Knopf für die Ansicht"**, um schnell wieder eine ausgewogene und übersichtliche Arbeitsfläche herzustellen.

* **💾 Snapshot (Grün):**
    * **Was es tut:** Erstellt ein umfassendes Backup des gesamten aktuellen Rosie-Zustands. Dies beinhaltet alle Code-Fenster, Historien, API-Schlüssel und Datenbanken in einem neuen Ordner im `CodeBackups`-Verzeichnis.
    * **Warum es existiert:** Dies ist Ihr **"digitaler Einfrierpunkt"**. Wenn Sie an einem komplexen Refactoring arbeiten, können Sie jederzeit einen Snapshot erstellen und im Notfall zu diesem sicheren Punkt zurückkehren.

* **🧹 Clear (Orange):**
    * **Was es tut:** Leert beide Code-Fenster ("Status Quo" und "Neues Update") vollständig.
    * **Warum es existiert:** Dies ist Ihr **"Labor-Besen"**. Der schnelle Weg zu einem sauberen Arbeitsbereich für einen neuen KI-Prompt.

### 2.3. Die zwei Code-Fenster: Ihr digitales Labor

Der obere Bereich von Rosie ist in zwei große RichTextBox-Fenster unterteilt:

* **Linkes Fenster: Status Quo (Alt)**
    * **Zweck:** Dies ist der **"Ankerpunkt in der Vergangenheit"**. Hier liegt der Code, der sich bewährt hat, kompiliert wurde und aktuell läuft.
    * **Interaktion:** Dieser Bereich ist schreibgeschützt, um unbeabsichtigte Änderungen zu verhindern.

* **Rechtes Fenster: Neues Update (Neu)**
    * **Zweck:** Dies ist Ihre **"aktive Werkbank"**. Hier landet der Code, den Sie bearbeiten oder von der KI generieren lassen.
    * **Interaktion:** Dieses Feld ist vollständig editierbar. Dieser Code ist die **"Zukunft"**, die Sie mit dem "⚡ Analyse"-Button prüfen.

* **Shift-to-Left (Checkbox im rechten Code-Fenster):**
    * **Zweck:** Steuert den **"Marathon-Flow"**. Wenn aktiviert, wird beim Einfügen von neuem Code der *aktuelle* Inhalt des rechten Fensters zuerst automatisch in das linke Fenster verschoben.
    * **Warum es existiert:** Spart manuelles Kopieren. Das letzte "gute" Update wird automatisch zum neuen "Status Quo".

### 2.4. Die unteren Tabs: Ihre Arbeitsbereiche auf einen Blick

* **🔍 Diff-Protokoll:** Zeigt die Ergebnisse der Code-Analyse.
* **📦 Architektur & Lexikon:** Interaktiver Syntaxbaum und KI-Erklärungen.
* **💾 Export:** Architektur (AST) als JSON exportieren.
* **✨ Seele-Motor:** KI beauftragen, die semantische Beschreibung zu erstellen.
* **⚙️ Einstellungen:** Pfade und Compiler konfigurieren.
* **💬 KI Chat:** Chat-Verläufe speichern und als Kontext nutzen.
* **🚀 KI-Inception:** Neue Projekte züchten, API konfigurieren und Code generieren.
* **🌌 Matrix (Raw API):** Rohe Antworten der KI-Server einsehen.
* **📘 Handbuch:** Handbücher oder Dokumentationen generieren.

## 3. Die Arbeitsbereiche im Detail

### 3.1. 🔍 Diff-Protokoll: Der Röntgenscanner für Code-Änderungen

Dieser Tab zeigt visuell, was sich zwischen "Status Quo" und "Neues Update" geändert hat:

* **Gelöschte Elemente (Orange):** ALARM für potenziell unabsichtlich entfernte Teile ("Stampf-Fehler").
* **Neu hinzugefügte Elemente (Hellgrün):** Zeigt neue Features oder Bauteile.
* **Verdrahtung (Dunkelgrau):** Zeigt bei Methoden, welche anderen Methoden intern aufgerufen werden.
* **"INITIAL COMMIT" (Türkis):** Signalisiert den Start eines neuen Projekts.
* **"Identisch" (Grün):** Grünes Licht für sichere Updates.
* **"Abweichung!" (Rot):** Stoppschild bei strukturellen Änderungen.

### 3.2. 📦 Architektur & Lexikon: Die Seele des Codes visualisieren

Dieser Tab ist Ihr **"digitaler Zwilling"**:

* **Der Abstrakte Syntaxbaum (AST):** Eine logische Landkarte Ihres Codes. Hierarchische Darstellung aller Klassen (`🏗️`), Felder (`🧩`) und Methoden (`⚙️`). Wird beim Tippen live aktualisiert.
* **Das Lexikon:** Die **"Seele"** des Codes. Zeigt für angeklickte Elemente den "Grund der Existenz" und die "Auswirkung bei Aktivierung" an.

### 3.3. 💾 Export: Sicherung Ihrer Code-Landkarten
Exportiert die Architekturbäume (ASTs) Ihres Codes als strukturierte `.json`-Dateien.

### 3.4. ✨ Seele-Motor: KI-Intelligenz für Ihre Dokumentation

* **"🌀 1. Seele anfordern":** Erstellt ein Delta-JSON für Code ohne Beschreibung und kopiert es samt System-Prompt in die Zwischenablage.
* **"📥 2. Seele integrieren":** Liest das "beseelte" JSON der KI aus der Zwischenablage und speichert es lokal ab.

### 3.5. ⚙️ Einstellungen: Das Kontrollzentrum

* **"1. Ziel-Datei":** Die `Program.cs`, in die Rosie den Code speichern soll.
* **"2. Eigenes Build-Skript":** Das `.bat`-Skript zum Kompilieren.
* **"3. Ziel-EXE":** Die ausführbare Datei, die nach dem Build gestartet werden soll.
* **Persistenz:** Rosie speichert alle Einstellungen, Fensterpositionen und Historien vollautomatisch beim Beenden in JSON-Dateien.

### 3.6. 💬 KI Chat: Das Gedächtnis Ihrer Kommunikation

Ein Bereich für historische KI-Chats, der als Kontext für neue Anfragen dient. Er unterstützt Autosave (alle 3 Sekunden) und das direkte Einlesen von JSON-Exporten (z.B. aus dem SaveAI Plugin).

### 3.7. 🚀 KI-Inception: Ihr autonomes Entwicklungsstudio

Das Herzstück von Rosie, Ihr persönlicher **"C# AutoDevin"**. 

* **"🌱 Projekt züchten & Wächter klonen"**
    Dieser Button ist die **"Geburt eines neuen Projekts"**. Rosie erstellt den Ordner, generiert die Struktur, installiert NuGets, schreibt die `build.bat` und klont sich selbst als neuer Wächter in das Projekt – inklusive "Zero-Config" für alle Pfade.
* **API-Konfiguration:**
    Tragen Sie Ihren API-Key ein, laden Sie Live-Modelle von Google, passen Sie den API-Link an und setzen Sie die Pause-Handbremse, um API-Limits zu schonen.
* **Der Inception-Workflow:**
    * `Dein Prompt an die Maschine:` Formulieren Sie Ihre Wünsche. Code und Chat-Historie werden unsichtbar angehängt.
    * **"📡 Senden via API":** Startet den **"Ping-Pong-Motor"**, umgeht Längenlimits, stückelt den Code nahtlos in das rechte Fenster und installiert bei Bedarf automatisch erkannte NuGets.
    * **"📦 Auto-NuGet Scanner":** Wirft alten Code in den Scanner und lässt die KI fehlende Paket-Abhängigkeiten ermitteln und via `dotnet add package` verankern.
    * **Fallback-Buttons:** Manueller "Rettungsanker" über die Zwischenablage, falls die direkte API blockiert wird.

### 3.8. 🌌 Matrix (Raw API): Die ungeschminkte Wahrheit

Zeigt die rohen, ungefilterten JSON-Antworten der Google-Server. Essentiell, um Status-Codes oder Limit-Überschreitungen (z.B. `RESOURCE_EXHAUSTED`) zu prüfen.

### 3.9. 📘 Handbuch: Das Handbuch der Handbücher

Ihr Kraftwerk für Dokumentationserstellung. 
* **Prompt-Bibliothek:** Verwalten und speichern Sie Vorlagen (System-Direktiven).
* **"📘 Handbuch generieren":** Erzeugt tiefgreifende Dokumentationen im Ping-Pong-Verfahren direkt aus Ihrem Code und dem Chat-Kontext.
* **"🔄 Raw JSON / Text":** Schaltet zwischen Markdown-Ansicht und dem echten JSON um (falls die KI die Syntax bricht, können Sie hier den Text retten).

## 4. Der Zyklus der Evolution: Ihr Workflow mit Rosie

### 4.1. Neues Projekt beginnen: Die Zucht eines Klons
Starten Sie die Mutter, füllen Sie Arbeitsverzeichnis und Projektname aus und züchten Sie mit einem Klick das komplett autarke Klon-Labor.

### 4.2. Ersten Code generieren und iterieren
Leeren Sie das Labor ("🧹 Clear"), formulieren Sie Ihren Prompt und senden Sie ihn ab. Analysieren Sie das Update ("⚡ Analyse") und drücken Sie auf Build ("🔨 Build & Neustart"). Wiederholen Sie den Zyklus, bis die Software perfekt ist.

### 4.3. Den Mutter-Wächter aktualisieren: Reverse-Inception
Nutzen Sie einen Klon, um die geschlossene Mutter zu patchen. Richten Sie die Pfade des Klons auf die Dateien der Mutter, fügen Sie das Update in den Klon ein, kompilieren Sie und starten Sie die aktualisierte Mutter neu.

### 4.4. Alte Projekte wiederbeleben: Der Code-Nekromant
Alten Code einwerfen, den "📦 Auto-NuGet Scanner" starten und zusehen, wie Rosie die fehlenden Pakete der Vergangenheit installiert.

## 5. FAQ: Häufig gestellte Fragen und Rosies Antworten

### 5.1. "Was ist der Roslyn Code-Wächter und warum sollte ich ihn nutzen?"
Rosie ist Ihr KI-Assistent und unbestechlicher Wächter, der Code überwacht, vor KI-Halluzinationen schützt und die Code-Generierung, das Dependency-Management und den Build automatisiert.

### 5.2. "Rosie startet nicht oder vergisst meine Einstellungen nach Neustart!"
Rosie muss zwingend aus ihrem eigenen Verzeichnis gestartet werden. Erfolgt der Aufruf aus einem falschen Arbeitsverzeichnis (z.B. durch Skripte), findet sie ihre `.json`-Gedächtnisdateien nicht.

### 5.3. "Der Analyse-Button zeigt nichts an, obwohl Code da ist."
Dies ist der "INITIAL COMMIT". In aktuellen Versionen listet Rosie alle Elemente aus dem rechten Fenster grün als "NEU" auf, auch wenn das linke Fenster komplett leer ist.

### 5.4. "Ich habe kompiliert, aber die EXE startet nicht die neue Version!"
Meist schuld: Der MSBuild Cache oder ein falscher EXE-Startpfad in den Einstellungen. 
*Lösung:* Rosie nutzt jetzt intern `dotnet build --no-incremental`. Überprüfen Sie nach dem Kompilieren, ob das System einen neuen Ordner angelegt hat (z.B. `net8.0-windows10.0.19041.0`) und passen Sie den "Ziel-EXE"-Pfad im Einstellungen-Tab neu an.

### 5.5. "Die KI-Antwort ist voller Tutorials und kein reiner Code!"
*Lösung:* Durch den "System-Maulkorb" von Rosie wird die KI inzwischen gezwungen, strikt C#-Code in das JSON-Fach zu schreiben. Markdown-Formatierungen der KI werden gefiltert. Erklärungen landen in der MessageBox.

### 5.6. "Senden via API schlägt mit 'Invalid URI' fehl."
Oft verursacht durch versteckte Leerzeichen oder Zeilenumbrüche im API-Key-Feld. Rosie nutzt das robuste `curl` Kommandozeilen-Tool, prüfen Sie dennoch den Key auf Sauberkeit.

### 5.7. "Ich sehe nur 'Flash'-Modelle, aber keine 'Pro'-Modelle."
Google limitiert den kostenlosen Pro-Tier drastisch (oft auf 32k Tokens pro Minute). Um große Projekt-Dateien an ein Pro-Modell zu schicken, müssen Sie in der Google Cloud Console meist "Pay-as-you-go" mit einer Kreditkarte aktivieren. Das hebt das Limit auf mehrere Millionen Tokens.

### 5.8. "Rosie installiert NuGet-Pakete nicht automatisch."
Der `dotnet add package` Befehl überspringt bereits vorhandene Pakete automatisch (Idempotenz). Die KI schreibt in der Regel nur fehlende Pakete in die Installationsanweisung.

### 5.9. "Das linke Code-Fenster aktualisiert sich nach dem Build nicht."
In der aktuellen Version kopiert Rosie den erfolgreich kompilierten Code automatisch in das linke ("Status Quo") Fenster. So bleibt die Referenz immer aktuell.

### 5.10. "Der mittlere Balken im AST-Explorer ist völlig verstellt."
Dies ist ein bekannter Windows Forms Render-Bug bei versteckten Tab-Reitern. Drücken Sie den "⚖️ Mitte"-Button in der oberen Leiste, um alle Balken auf saubere 50% zurückzusetzen.

### 5.11. "Das Diff-Protokoll verliert nach Neustart seine Farben."
Seit v15 speichert Rosie den Log im Rich Text Format (RTF). Warnungen in Rot und neue Features in Grün bleiben für die Ewigkeit erhalten.

### 5.12. "Fehlende Referenzen nach Einfügen von altem Code."
Benutzen Sie den "📦 Auto-NuGet Scanner" im KI-Inception Tab.

### 5.13. "Target Framework Fehler bei Nutzung nativer Windows APIs."
Wenn die KI Windows-spezifischen Code generiert (z.B. Media-Player oder OCR), muss das Projekt das entsprechende Windows SDK referenzieren.
*Lösung:* Öffnen Sie Ihre `.csproj` und ändern Sie `<TargetFramework>net8.0-windows</TargetFramework>` manuell zu `<TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>`. Danach kompilieren Sie erneut.

---
Das war Ihr umfassendes Handbuch zum Roslyn Code-Wächter. Viel Erfolg beim Züchten Ihrer Projekte!
