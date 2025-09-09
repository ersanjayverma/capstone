package com.capstone.Arogya.model;

import jakarta.persistence.*;
import lombok.*;

import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "goals")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
@ToString(exclude = {"user", "activities"})
public class Goal {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @EqualsAndHashCode.Include
    private Long id;

    @Column(nullable = false)
    private String title;

    private String description;

    private boolean completed;

    @Column(name = "progress_percentage", nullable = false)
    private double progressPercentage = 0.0;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", nullable = false)
    private User user;

    @OneToMany(mappedBy = "goal", 
        cascade = CascadeType.ALL, 
        orphanRemoval = true, 
        fetch = FetchType.LAZY)
    private Set<Activity> activities = new HashSet<>();

    public void recomputeProgress() {
        if (activities == null || activities.isEmpty()) {
            this.progressPercentage = this.completed ? 100.0 : 0.0;
            return;
        }

        long doneCount = activities.stream().filter(Activity::isDone).count();
        double pct = (doneCount * 100.0) / activities.size();
        // clamp to [0.0, 100.0]
        if (pct < 0.0) pct = 0.0;
        if (pct > 100.0) pct = 100.0;
        this.progressPercentage = pct;
    }
}
